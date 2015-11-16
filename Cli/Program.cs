using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler;
using System.IO;
using Microsoft.CodeAnalysis.MSBuild;
namespace CompiledHandlebars.Cli
{
  class Program
  {
    static void Main(string[] args)
    {
      if (args.Length!=1)
      {
        Usage();
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
      
      foreach(var proj in solution.Projects)
      {
        foreach(var addFile in proj.AdditionalDocuments.Where(x => Path.GetExtension(x.FilePath).Equals(".hbs")))
        {
          //TODO: Get namespace, compile Handlebars template, write resulting code back to solution 
        }
      }
    }

    private static void Usage()
    {
      Console.WriteLine("Usage: HandlebarsCompiler.exe SolutionFile");
    }

    private static void FileDoesNotExist(string solutionFile)
    {
      Console.WriteLine($"File '{solutionFile}' does not exist!");
    }
  }
}
