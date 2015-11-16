using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler;
using System.IO;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace CompiledHandlebars.Cli
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length!=1)
      {
        ShowUsage();
        return;
      }
            
      var solutionFile = args[0];
      if (!File.Exists(solutionFile))
      {
        FileDoesNotExist(solutionFile);
        return;
      }
      CompileAllHandlebarsTemplates(solutionFile);
    }

    private static void CompileAllHandlebarsTemplates(string solutionFile)
    {
      var properties = new Dictionary<string, string>() {
        { "AdditionalFileItemNames", "none" }};
      var workspace = MSBuildWorkspace.Create(properties);
      var solution = workspace.OpenSolutionAsync(solutionFile).Result;
      Console.WriteLine($"Searching {solution.FilePath} for HandlebarsTemplates");
      foreach(var projectId in solution.ProjectIds)
      {
        var project = solution.GetProject(projectId);
        Console.WriteLine($"Searching {project.Name} for HandlebarsTemplates");
        var projectBaseNamespace = string.Join(".", project.GetCompilationAsync().Result.Assembly.NamespaceNames.Where(x => !string.IsNullOrEmpty(x)));
        foreach (var addFile in project.AdditionalDocuments.Where(x => Path.GetExtension(x.FilePath).Equals(".hbs")))
        {
          Console.WriteLine($"Found '{addFile.Name}'");
          //TODO: Get namespace, compile Handlebars template, write resulting code back to solution 
          var templateNamespace = string.Join(".", projectBaseNamespace, string.Join(".", addFile.Folders));
          var compilationResult = HbsCompiler.Compile(addFile.GetTextAsync().Result.ToString(), templateNamespace, Path.GetFileNameWithoutExtension(addFile.Name), project);
          //Check if template already exits
          if (compilationResult.Item2.Any())
          {//Output errors
            Console.WriteLine($"HandlebarsTemplate '{addFile.Name}' threw following errors:");
            foreach(var error in compilationResult.Item2)
            {
              Console.WriteLine(error.Message);
              Console.WriteLine(addFile.GetTextAsync().Result.Lines[error.Line-1].ToString());
            }
          }
          else
          {//Save file to project
            var doc = project.Documents.FirstOrDefault(x => x.Name.Equals(string.Concat(addFile, ".cs")));
            if (doc != null)
            {//And change it if it does
              project = doc.WithSyntaxRoot(CSharpSyntaxTree.ParseText(SourceText.From(compilationResult.Item1)).GetRoot()).Project;
            }
            else
            {//Otherwise add a new document
              project = project.AddDocument(string.Concat(addFile, ".cs"), compilationResult.Item1, addFile.Folders).Project;
            }
            workspace.TryApplyChanges(project.Solution);
          }
        }
      }
    }

    private static void ShowUsage()
    {
      Console.WriteLine("Usage: HandlebarsCompiler.exe SolutionFile");
    }

    private static void FileDoesNotExist(string solutionFile)
    {
      Console.WriteLine($"File '{solutionFile}' does not exist!");
    }
  }
}
