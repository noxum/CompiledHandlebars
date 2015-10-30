using Microsoft.CodeAnalysis;
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
      ContainingProject = project.Id;
      if (workspace == null)
        workspace = project.Solution.Workspace;
      if (solution == null)
      {
        solution = workspace.CurrentSolution;
        foreach (var projectId in solution.ProjectIds.Where(x => !projectCompilations.ContainsKey(x)))
        {
          Compilation comp;
          if (solution.GetProject(projectId).TryGetCompilation(out comp))
            projectCompilations.Add(projectId, comp);
          else
            projectCompilations.Add(projectId, solution.GetProject(projectId).GetCompilationAsync().Result);
        }
      } else
      {
        var changes = project.Solution.GetChanges(solution);
        foreach (var addedProject in changes.GetAddedProjects())
          projectCompilations.Add(addedProject.Id, GetCompilationForProject(addedProject));
        foreach (var removedProject in changes.GetRemovedProjects())
          projectCompilations.Remove(removedProject.Id);
        foreach (var projectChanges in changes.GetProjectChanges())
          //Bruteforce way: Just get the new compilation...
          //If that does not scale try adding documents to the compilation (incremental update)
          projectCompilations[projectChanges.ProjectId] = GetCompilationForProject(projectChanges.NewProject);
        solution = project.Solution;
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
      return projectCompilations.Values.Select(x => x.GetTypeByMetadataName(fullTypeName)).Where(x => x != null).FirstOrDefault();
    }

    public bool RuntimeUtilsReferenced()
    {
      //TODO: Not only check if the type exists... but also if it is the correct version and supports everything needed from it
      return projectCompilations[ContainingProject].GetTypeByMetadataName("CompiledHandlebars.RuntimeUtils.RenderHelper") != null;      
    }

    public INamedTypeSymbol GetPartialHbsTemplate(string templateName)
    {
      foreach(var comp in projectCompilations.Values)
      {
        var allSymbols = comp.GetSymbolsWithName(x => true);
        var symbols = comp.GetSymbolsWithName(x => x.Contains(templateName));
        INamedTypeSymbol template = comp.GetSymbolsWithName(x => x.Equals(templateName)).OfType<INamedTypeSymbol>().FirstOrDefault(x => x.GetAttributes().Any(y => y.AttributeClass.Name.Equals("CompiledHandlebarsTemplateAttribute")));
        if (template != null)
          return template;
      }
      return null;
    }
    
  }
}
