using CompiledHandlebars.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Workspaces.Dnx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CompiledHandlebars.Cli
{
  public class Program
  {
    private static readonly char[] validFlags = {'n', 'f'};
    private class CompilerOptions
    {
      public string SolutionFile { get; set; }
      public string ProjectFile { get; set; }
      public string HandlebarsFile { get; set; }
      public string Namespace { get; set; }
      public List<string> DirectoryBlacklist { get; set; } = new List<string>();
      public List<string> DirectoryWhitelist { get; set; } = new List<string>();
      public bool JSONProject { get; set; }
      public bool DryRun { get; set; }
      public bool ForceRecompilation { get; set; }
    }

    public static void Main(string[] args)
    {
      var options = new CompilerOptions();      
      if (!args.Any())
      {
        ShowUsage();
        return;
      }
      if (IsValidFlagArgument(args[0]))
      {
        foreach(var chr in args[0].Skip(1))
        {
          switch(chr)
          {
            case 'f': options.ForceRecompilation = true; break;
            case 'n': options.DryRun = true; break;
            default: ShowUsage(); return;
          }
        }
        args = args.Skip(1).ToArray();
      }
      if (args.Length > 1)
      {//Handle blacklisted and whitelisted directories
        foreach(var arg in args.Take(args.Length -1))
        {
          if (arg.StartsWith("-e"))
            options.DirectoryBlacklist.Add(arg.Substring(2));
          else if (arg.StartsWith("-i"))
            options.DirectoryWhitelist.Add(arg.Substring(2));
          else
          {
            ShowUsage();
            return;
          }
        }
        args = args.Skip(args.Length - 1).ToArray();
      }
      if (args.Length==1)
      {
        if (!File.Exists(args[0]))
        {
          FileDoesNotExist(args[0]);
          return;
        }
        string extension = Path.GetExtension(args[0]);
        if (extension.Equals(".csproj"))
        {
          options.ProjectFile = args[0];
          CompileProject(options);
        }
        else if (extension.Equals(".json"))
        {
          options.ProjectFile = args[0];
          options.JSONProject = true;
          CompileProject(options);
        }
        else if (extension.Equals(".sln"))
        {
          options.SolutionFile = args[0];
          CompileSolution(options);
        } else
        {

        }
      } else
      {
        ShowUsage();
        return;
      }
    }

    private static bool IsValidFlagArgument(string arg)
    {
      return (arg.StartsWith("-") && arg.Skip(1).All(x => validFlags.Contains(x)));
    }


    private static void PrintUnknownExtension(string ext)
    {
      Console.WriteLine($"Unknown file extension '{ext}'. The compiler accepts solution files (.sln) or project files (.csproj or .json)");
    }

    private static void CompileProject(CompilerOptions options)
    {
      Project project;
      Workspace workspace;
      var handlebarsFiles = new List<string>();
      if (options.JSONProject)
      {
        workspace = new ProjectJsonWorkspace(options.ProjectFile);
        project = workspace.CurrentSolution.Projects.First();
        handlebarsFiles.AddRange(ScrapeDirectoryForHandlebarsFiles(new DirectoryInfo(Path.GetDirectoryName(project.FilePath)), options));        
      } else
      {//Old project files. Accessible via MSBuildWorkspace
        var properties = new Dictionary<string, string>() {
        { "AdditionalFileItemNames", "none" }};
        workspace = MSBuildWorkspace.Create(properties);
        project = (workspace as MSBuildWorkspace).OpenProjectAsync(options.ProjectFile).Result;
        handlebarsFiles.AddRange(project.AdditionalDocuments.Where(x => Path.GetExtension(x.FilePath).Equals(".hbs")).Select(x => x.FilePath));
      }
      CompileHandlebarsFiles(project, workspace, handlebarsFiles, options);
    }

    private static List<string> ScrapeDirectoryForHandlebarsFiles(DirectoryInfo directory, CompilerOptions options, bool recursive = true)
    {
      var result = new List<string>();
      foreach(var file in directory.EnumerateFiles())
      {
        if (file.Extension.Equals(".hbs"))
          result.Add(file.FullName);
      }
      if (recursive)
      {
        if (options.DirectoryWhitelist.Any())
        {//is there a directory whitelist?
          foreach (var subDir in directory.EnumerateDirectories().Where(x => options.DirectoryWhitelist.Contains(x.Name)))
          {
            result.AddRange(ScrapeDirectoryForHandlebarsFiles(subDir, options));
          }
        } else
        {
          foreach (var subDir in directory.EnumerateDirectories().Where(x => !options.DirectoryBlacklist.Contains(x.Name)))
          {
            result.AddRange(ScrapeDirectoryForHandlebarsFiles(subDir, options));
          }
        }
      }
      return result;
    }

    private static void CompileSolution(CompilerOptions options)
    {
      var properties = new Dictionary<string, string>() {
        { "AdditionalFileItemNames", "none" }};
      var workspace = MSBuildWorkspace.Create(properties);
      var solution = (workspace as MSBuildWorkspace).OpenSolutionAsync(options.SolutionFile).Result;
      foreach(var projectId in solution.ProjectIds)
      {
        var project = workspace.CurrentSolution.Projects.First(x => x.Id.Equals(projectId));
        var handlebarsFiles = project.AdditionalDocuments.Where(x => Path.GetExtension(x.FilePath).Equals(".hbs")).Select(x => x.FilePath).ToList();
        if (handlebarsFiles.Any())
        {
          workspace = CompileHandlebarsFiles(project, workspace, handlebarsFiles, options) as MSBuildWorkspace;          
        }
      }
    }

    private static Workspace CompileHandlebarsFiles(Project project, Workspace workspace, List<string> hbsFiles, CompilerOptions options)
    {
      bool successFullCompilation = true;      
      while(hbsFiles.Any() && successFullCompilation)
      {
        successFullCompilation = false;
        var nextRound = new List<string>();
        foreach(var file in hbsFiles)
        {
          var fileInfo = new FileInfo(file);
          string @namespace;
          bool compiledVersionExists = File.Exists($"{file}.cs");
          bool compiledVersionIsOlder = true;
          if (compiledVersionExists)
          {//Compiled Version already exists
            var compiledFileInfo = new FileInfo($"{file}.cs");
            compiledVersionIsOlder = (fileInfo.LastWriteTimeUtc > compiledFileInfo.LastWriteTimeUtc);
            @namespace = DetermineNamespace(compiledFileInfo);
          } else
          {
            @namespace = DetermineNamespace(fileInfo, project);
          }
          if (compiledVersionIsOlder || options.ForceRecompilation)
          {
            string content = File.ReadAllText(file);       
            string name = Path.GetFileNameWithoutExtension(file);
            var compilationResult = CompileHandlebarsTemplate(content, @namespace, name, project, options);
            if (!options.DryRun)
            {
              if (compilationResult?.Item2?.Any() ?? false)
              {//Errors occured
                if (compilationResult.Item2.OfType<HandlebarsTypeError>().Any(x => x.Kind == HandlebarsTypeErrorKind.UnknownPartial))
                {//Unresolvable Partial... could be due to compiling sequence
                  //Console.WriteLine($"Unresolved partial call for template '{name}'. Try again!");
                  nextRound.Add(file);
                }
                else
                  foreach (var error in compilationResult.Item2)
                    PrintError(error);
              }
              else
              {
                successFullCompilation = true;
                //Check if template already exits
                var doc = project.Documents.FirstOrDefault(x => x.Name.Equals(string.Concat(name, ".hbs.cs")));
                if (doc != null)
                {//And change it if it does
                  project = doc.WithSyntaxRoot(CSharpSyntaxTree.ParseText(SourceText.From(compilationResult.Item1)).GetRoot()).Project;
                }
                else
                {//Otherwise add a new document
                  project = project.AddDocument(string.Concat(name, ".hbs.cs"), SourceText.From(compilationResult.Item1), GetFolderStructureForFile(fileInfo, project)).Project;
                }
                try {
                  if (workspace.TryApplyChanges(project.Solution))
                    Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++");
                  else
                    Console.WriteLine("---------------------------------------------");
                  project = workspace.CurrentSolution.Projects.First(x => x.Id.Equals(project.Id));
                } catch(NotSupportedException)
                {//ProjectJsonWorkspace does not support adding documents (as of 2016-02-17). So just add it manually
                  File.WriteAllText($"{file}.cs", compilationResult.Item1);
                }
              }
            }
          }    
        }
        hbsFiles = nextRound;
      }
      return workspace;
    }
        
    private static void PrintError(HandlebarsException error)
    {
      Console.WriteLine($"Compilation failed: {error.Message}");
    }

    private static Tuple<string, IEnumerable<HandlebarsException>> CompileHandlebarsTemplate(string content, string @namespace, string name, Project containingProject, CompilerOptions options)
    {
      if (options.DryRun)
      {
        Console.WriteLine($"Compile file '{name}' in namespace '{@namespace}'");
        return null;
      } else
      {
        Console.WriteLine($"Compiling '{name}'...");
        return HbsCompiler.Compile(content, @namespace, name, containingProject);
      }
    }


    private static string DetermineNamespace(FileInfo compiledTemplate)
    {
      var code = File.ReadAllText(compiledTemplate.FullName);
      SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
      var root = (CompilationUnitSyntax)tree.GetRoot();
      return root.Members.OfType<NamespaceDeclarationSyntax>().First().Name.ToString();
    }

    private static string DetermineNamespace(FileInfo hbsFile, Project containingProject)
    {
      string templateDir = Path.GetDirectoryName(hbsFile.FullName);
      string projectDir = Path.GetDirectoryName(containingProject.FilePath);
      return string.Concat(containingProject.AssemblyName, templateDir.Substring(projectDir.Length).Replace(Path.DirectorySeparatorChar, '.'));
    }

    private static IEnumerable<string> GetFolderStructureForFile(FileInfo file, Project containingProject)
    {
      string fileDir = Path.GetDirectoryName(file.FullName);
      string projectDir = Path.GetDirectoryName(containingProject.FilePath);
      return fileDir.Substring(projectDir.Length).Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static void ShowUsage()
    {
      Console.WriteLine("Usage: HandlebarsCompiler.exe -[flags] ([-e<Excluded Directory>]*|[-i<Included Directory>]*)  <SolutionFile>|<ProjectFile>");
      Console.WriteLine("Flags:");
      Console.WriteLine("      -n");
      Console.WriteLine("         Dry-Run: don't actually compile anything, just show what would be done ");
      Console.WriteLine("      -f");
      Console.WriteLine("         Force Compilation: compile handlebars-file even if it did not change");
      Console.WriteLine("");
      Console.WriteLine("Directory Black- and Whitelists:");
      Console.WriteLine("-e<Exluded Directory> excludes a directory from compilation. Handlebars files in this folder will be ignored. Multiple statements possible.");
      Console.WriteLine("-i<Included Directory> includeds a directory from compilation. Only Handlebars files in this folder will be compiled. Multiple statements possible.");
    }

    private static void FileDoesNotExist(string solutionFile)
    {
      Console.WriteLine($"File '{solutionFile}' does not exist!");
    }
  }
}
