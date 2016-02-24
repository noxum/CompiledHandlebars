using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;

namespace CompiledHandlebars.Compiler.Introspection
{
  public class RoslynIntrospector
  {
    private static Workspace workspace { get; set; }
    private static Solution solution { get; set; }
    //Project which contains the HandlebarsTemplate to be compiled
    private static ProjectId ContainingProject { get; set; }
    private static Dictionary<ProjectId, Compilation> projectCompilations { get; set; } = new Dictionary<ProjectId, Compilation>();
    
    //TODO: Test what happens if multiple instances of VisualStudio run...
    public RoslynIntrospector(Project project)
    {
      if (workspace == null)
        workspace = project.Solution.Workspace;
      if (solution == null)
      {
        solution = workspace.CurrentSolution;
        UpdateProjectCompilations(project);
      } else if (ContainingProject.Equals(project.Id))
      {
        var changes = project.Solution.GetChanges(solution);        
        foreach (var removedProject in changes.GetRemovedProjects())
          projectCompilations.Remove(removedProject.Id);
        foreach (var addedProject in changes.GetAddedProjects())
          projectCompilations.Add(addedProject.Id, GetCompilationForProject(addedProject));
        foreach (var projectChanges in changes.GetProjectChanges())
          //Bruteforce way: Just get the new compilation...
          //If that does not scale try adding documents to the compilation (incremental update)
          projectCompilations[projectChanges.ProjectId] = GetCompilationForProject(projectChanges.NewProject);
        solution = project.Solution;
      } else
      {//Solution and workspace does not change but project does and so do the dependencies. 
       //We then need different projects in the projectCompilations Dictionary
        UpdateProjectCompilations(project);
        workspace = project.Solution.Workspace;
        solution = workspace.CurrentSolution;
      }
      ContainingProject = project.Id;
    }

    private void UpdateProjectCompilations(Project project)
    {
      projectCompilations = new Dictionary<ProjectId, Compilation>();
      projectCompilations[project.Id] = GetCompilationForProject(project);
      foreach (var projectId in solution.GetProjectDependencyGraph().GetProjectsThatThisProjectDirectlyDependsOn(project.Id))
      {
        projectCompilations[projectId] = GetCompilationForProject(solution.GetProject(projectId));
      }
    }

    private Compilation GetCompilationForProject(Project proj)
    {
      Compilation comp;
      if (proj.TryGetCompilation(out comp))
        return comp;
      else return proj.GetCompilationAsync().Result;      
    }

    public INamedTypeSymbol GetTypeSymbol(string fullTypeName)
    {
      //Usual Namespace: Name.Space.Class
      var symbol = projectCompilations.Values.Select(x => x.GetTypeByMetadataName(fullTypeName)).Where(x => x != null).FirstOrDefault();
      if (symbol != null)
        return symbol;
      //Maybe its a nested class... try out Name.Space+Class      
      //TODO: Think of something more clever. i.e. Check if Name.Space exists and than look for nested type Class
      while(fullTypeName.LastIndexOf('.')>=0)
      {
        //Wow. That sucks...
        int index = fullTypeName.LastIndexOf('.');
        fullTypeName = fullTypeName.Remove(index, 1);
        fullTypeName = fullTypeName.Insert(index, "+");
        symbol = projectCompilations.Values.Select(x => x.GetTypeByMetadataName(fullTypeName)).Where(x => x != null).FirstOrDefault();
        if (symbol != null)
          return symbol;
      }
      return null;
    }

    public bool RuntimeUtilsReferenced()
    {
      //TODO: Not only check if the type exists... but also if it is the correct version and supports everything needed from it
      return projectCompilations[ContainingProject].GetTypeByMetadataName("CompiledHandlebars.RuntimeUtils.RenderHelper") != null;      
    }

    public INamedTypeSymbol GetPartialHbsTemplate(string templateName)
    {
      return FindClassesWithNameAndAttribute(templateName, StringConstants.TEMPLATEATTRIBUTEFULL);
    }

    public INamedTypeSymbol GetLayoutHbsTemplate(string layoutName)
    {
      return FindClassesWithNameAndAttribute(layoutName, StringConstants.LAYOUTATTRIBUTEFULL);
    }

    private INamedTypeSymbol FindClassesWithNameAndAttribute(string fullName, string attribute)
    {
      var name = fullName.Split('.').Last();
      foreach (var comp in projectCompilations.Values)
      {
        INamedTypeSymbol template = comp.GetSymbolsWithName(x => x.Equals(name, System.StringComparison.Ordinal))
                                        .OfType<INamedTypeSymbol>()
                                        .Where(x => NamespaceUtility.IsPartOf(x.ToDisplayString(), fullName))                                                                                
                                        .FirstOrDefault(x => x.GetAttributes()
                                                              .Any(y => y.AttributeClass.Name.Equals(attribute)));
        if (template != null)
          return template;
      }
      return null;
    }

    /// <summary>
    /// Searches each referenced project for helper methods. 
    /// These must serve following conditions:
    ///   - correct name
    ///   - be static
    ///   - have a "CompiledHandlebarsHelperMethod" attribute
    ///   - have matching parameters
    /// </summary>
    /// <param name="funtionName">Name of the Helper as declared in the handlebars-template</param>
    /// <param name="parameters">Types of the Parameters that are passed to the helper method</param>
    /// <returns>The MethodSymbol for the called HelperMethod or null if it could not be found</returns>
    public IMethodSymbol GetHelperMethod(string funtionName, List<ITypeSymbol> parameters)
    {
      foreach(var comp in projectCompilations.Values)
      {
        var candidates =  comp.GetSymbolsWithName(x => x.Equals(funtionName))
                                          .OfType<IMethodSymbol>()
                                          .Where(x => x.IsStatic &&
                                                      x.GetAttributes().Any(y => y.AttributeClass.Name.Equals(StringConstants.HELPERMETHODATTRIBUTEFULL)));
        var helperMethod = candidates.FirstOrDefault(x => DoParametersMatch(x, parameters));
        if (helperMethod != null)
          return helperMethod;
      }
      return null;
    }    

    private static bool DoParametersMatch(IMethodSymbol methodSymbol, List<ITypeSymbol> parameters)
    {
      if (methodSymbol.Parameters.Count() != parameters.Count)
        return false;
      for (int i = 0; i < methodSymbol.Parameters.Count(); i++)
      {
        if (!DoesParameterMatchType(methodSymbol.Parameters[i],parameters[i]))
          return false;
      }
      return true;
    }

    private static bool DoesParameterMatchType(IParameterSymbol param, ITypeSymbol type)
    {
      if (param.Type.Equals(type))
        return true;
      if (type.BaseType!=null && DoesParameterMatchType(param, type.BaseType))
        return true;
      if (param.Type.TypeKind == TypeKind.Interface)
        return type.AllInterfaces.Any(x => x.Equals(param.Type));
      if (param.Type.SpecialType.Equals(type.SpecialType))
        return true;
      return false;
    }

    public INamedTypeSymbol GetIntTypeSymbol()
    {
      return projectCompilations.First().Value.GetSpecialType(SpecialType.System_Int32);
    }

    public INamedTypeSymbol GetBoolTypeSymbol()
    {
      return projectCompilations.First().Value.GetSpecialType(SpecialType.System_Boolean);
    }

    public INamedTypeSymbol GetStringTypeSymbol()
    {
      return projectCompilations.First().Value.GetSpecialType(SpecialType.System_String);
    }
  }
}
