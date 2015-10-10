using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler.Introspection
{
  public class RoslynIntrospector
  {
    private readonly Solution _solution;
    private List<Compilation> projectCompilations { get; set; } = new List<Compilation>();
    public RoslynIntrospector(string solutionPath)
    {
      MSBuildWorkspace workspace = MSBuildWorkspace.Create();
      _solution = workspace.OpenSolutionAsync(solutionPath).Result;

      var taskList = _solution.Projects.Select(x => x.GetCompilationAsync());
      projectCompilations.AddRange(taskList.Select(x => x.Result));
    }

    public INamedTypeSymbol GetTypeSymbol(string fullTypeName)
    {
      return projectCompilations.Select(x => x.GetTypeByMetadataName(fullTypeName)).Where(x => x != null).FirstOrDefault();
    }
  }
}
