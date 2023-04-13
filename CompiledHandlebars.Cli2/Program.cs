using CompiledHandlebars.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Buildalyzer.Workspaces;
using Buildalyzer;
using Buildalyzer.Environment;
using Microsoft.Extensions.Logging;

namespace CompiledHandlebars.Core.Cli
{
    public class Program
    {
        private static readonly char[] validFlags = { 'n', 'f', 'c', 'd', 'w' };
        private static CompilerOptions _options;
        private static Workspace _workspace;
        private static Project _project;
        private static int _ExitCode = 0;

        private class CompilerOptions
        {
            public string SolutionFile { get; set; }
            public string ProjectFile { get; set; }
            public string HandlebarsFile { get; set; }
            public string Namespace { get; set; }
            public List<string> DirectoryBlacklist { get; set; } = new List<string>();
            public List<string> DirectoryWhitelist { get; set; } = new List<string>();
            public bool NetCoreProject { get; set; }
            public bool Debug { get; set; }
            public bool DryRun { get; set; }
            public bool Watch { get; set; }
            public bool ForceRecompilation { get; set; }
        }

        public static int Main(string[] args)
        {
            try
            {
                var options = new CompilerOptions();
                if (!args.Any())
                {
                    ShowUsage();
                    return -1;
                }
                while (IsValidFlagArgument(args[0]))
                {  // If there are flags, handle them. We already checked, that they are valid
                    foreach (var chr in args[0].Skip(1))
                    {
                        switch (chr)
                        {
                            case 'c': options.NetCoreProject = true; break;
                            case 'f': options.ForceRecompilation = true; break;
                            case 'n': options.DryRun = true; break;
                            case 'd': options.Debug = true; break;
                            case 'w': options.Watch = true; break;
                            default: ShowUsage(); return -1;
                        }
                    }
                    // Remove the flag from the arguments list
                    args = args.Skip(1).ToArray();
                }
                options.ForceRecompilation = true;
                // We have more than one argument left. So there must be some black/whitelisting going on!
                if (args.Length > 1)
                {//Handle blacklisted and whitelisted directories
                    foreach (var arg in args.Take(args.Length - 1))
                    {
                        if (arg.StartsWith("-e"))
                            // Exclude directory						
                            options.DirectoryBlacklist.Add(
                                Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), arg.Substring(2))));
                        else if (arg.StartsWith("-i"))
                            // Include directory
                            options.DirectoryWhitelist.Add(
                                Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), arg.Substring(2))));
                        else
                        {
                            // Some argument we do not know. Tell the user what to do!
                            ShowUsage();
                            return -1;
                        }
                    }
                    // All but the last argument is handled
                    args = args.Skip(args.Length - 1).ToArray();
                }
                //The last argument is the solution/project file
                if (args.Length == 1)
                {
                    //Make sure it exists
                    if (!File.Exists(args[0]))
                    {
                        FileDoesNotExist(args[0]);
                        return -1;
                    }
                    // Check if we have a project or a solution file
                    string extension = Path.GetExtension(args[0]);
                    if (extension.Equals(".csproj"))
                    {
                        options.ProjectFile = args[0];
                        CompileProject(options);
                    }
                    else if (extension.Equals(".sln"))
                    {
                        options.SolutionFile = args[0];
                        CompileSolution(options);
                    }
                    else
                    {
                        PrintUnknownExtension(extension);
                    }
                    Console.WriteLine("Done!");
                }
                else
                {
                    ShowUsage();
                    return -1;
                }
            }
            catch (Exception outerEx)
            {
                Console.WriteLine(outerEx);
                return -1;
            }
            Console.WriteLine($"Exit-Code: {_ExitCode}");
            return _ExitCode;
        }

        /// <summary>
        /// Check if string is a valid flag.
        /// Valid flags start with '-' and are known to the program 
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static bool IsValidFlagArgument(string arg)
        {
            return (arg.StartsWith("-") && arg.Skip(1).All(x => validFlags.Contains(x)));
        }


        private static void PrintUnknownExtension(string ext)
        {
            Console.WriteLine($"Unknown file extension '{ext}'. The compiler accepts solution files (.sln) or project files (.csproj)");
        }

        private static void CompileProject(CompilerOptions options)
        {
            Console.WriteLine($"Loading project '{options.ProjectFile}'...");
            //string x = Console.ReadLine();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var properties = new Dictionary<string, string>() {
                { "AdditionalFileItemNames", "none" }
            };

            Workspace workspace = null;
            Project project = null;
            List<string> handlebarsFiles;
            if (options.NetCoreProject)
            {
                StringWriter log = new StringWriter();
                try
                {
                    AnalyzerManagerOptions analyzerOptions = new AnalyzerManagerOptions
                    {
                        LogWriter = log
                    };
                    AnalyzerManager manager = new AnalyzerManager(analyzerOptions);

                    IProjectAnalyzer analyzer = manager.GetProject(options.ProjectFile);

                    workspace = analyzer.GetWorkspace();
                    //ILogger logger = manager.LoggerFactory?.CreateLogger<AdhocWorkspace>();
                    //workspace = new AdhocWorkspace();
                    //workspace.WorkspaceChanged += (sender, args) => logger?.LogDebug($"Workspace changed: {args.Kind.ToString()}{System.Environment.NewLine}");
                    //workspace.WorkspaceFailed += (sender, args) => logger?.LogError($"Workspace failed: {args.Diagnostic}{System.Environment.NewLine}");
                    //analyzer.Build(new EnvironmentOptions() { Restore = true }).FirstOrDefault().AddToWorkspace(workspace);

                    project = workspace.CurrentSolution.Projects.ElementAt(0);
                    sw.Stop();
                    Console.WriteLine($"Ok! {sw.Elapsed}");
                    Console.WriteLine($"Log: {log.ToString()}");
                    handlebarsFiles = new DirectoryInfo(Path.GetDirectoryName(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), options.ProjectFile)))).GetFiles("*.hbs", SearchOption.AllDirectories).Where(f => ShouldCompileFile(f.FullName, options)).Select(f => f.FullName).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Following errors occured: {Environment.NewLine}{ex} - {log.ToString()}");
                    throw;
                }
            }
            else
            {
                throw new NotImplementedException();
            }
            if (handlebarsFiles.Any())
            {
                CompileHandlebarsFiles(project, workspace, handlebarsFiles, options);
            }
            else
            {
                _ExitCode = -1;
                NoTemplates();
            }
            if (options.Watch)
            {
                _options = options;
                _workspace = workspace;
                //_project = project;
                var watcher = new FileSystemWatcher(Path.GetDirectoryName(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), options.ProjectFile))), "*.hbs");
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                             | NotifyFilters.FileName | NotifyFilters.DirectoryName;

                watcher.Changed += new FileSystemEventHandler(OnChanged);
                watcher.Created += new FileSystemEventHandler(OnChanged);
                watcher.Deleted += new FileSystemEventHandler(OnChanged);

                watcher.IncludeSubdirectories = true;
                watcher.EnableRaisingEvents = true;

                Console.WriteLine("Started watcher! \'q\' to quit.");
                while (Console.Read() != 'q') ;
            }
        }

        /// <summary>
        /// This method is called when a .hbs file changes!
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            if (ShouldCompileFile(e.FullPath, _options))
            {
                Console.WriteLine("Change detected!");
                _workspace = CompileHandlebarsFiles(_project, _workspace, new List<string> { e.FullPath }, _options);
                var project = _workspace.CurrentSolution.Projects.First(x => x.Id.Equals(_project.Id));
                _project = project;
            }
        }

        /// <summary>
        /// Compiles all Handlebars-Files in a Solution.
        /// </summary>
        /// <param name="options"></param>
        private static void CompileSolution(CompilerOptions options)
        {
            Console.WriteLine($"Loading solution '{options.SolutionFile}'...");
            var properties = new Dictionary<string, string>() {
                { "AdditionalFileItemNames", "none" }
            };

            Workspace workspace = null;
            Solution solution = null;
            if (options.NetCoreProject)
            {
                try
                {
                    AnalyzerManager manager = new AnalyzerManager(options.SolutionFile);
                    workspace = manager.GetWorkspace();
                    Console.WriteLine("Ok!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Following errors occured: {Environment.NewLine}{ex}");
                    throw;
                }
            }
            else
            {
                throw new NotImplementedException();
            }

            solution = workspace.CurrentSolution;
            bool anyHbsFilesFound = false;
            foreach (var projectId in solution.ProjectIds)
            {
                var project = workspace.CurrentSolution.Projects.First(x => x.Id.Equals(projectId));
                List<string> handlebarsFiles;
                if (options.NetCoreProject)
                {
                    string projectFile = project.FilePath;
                    handlebarsFiles = new DirectoryInfo(Path.GetDirectoryName(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), projectFile)))).GetFiles("*.hbs", SearchOption.AllDirectories).Where(f => ShouldCompileFile(f.FullName, options)).Select(f => f.FullName).ToList();
                    if (handlebarsFiles.Count > 0)
                        anyHbsFilesFound = true;
                }
                else
                {
                    handlebarsFiles = project.AdditionalDocuments.Where(x => Path.GetExtension(x.FilePath).Equals(".hbs")).Select(x => x.FilePath).Where(x => ShouldCompileFile(x, options)).ToList();
                    if (handlebarsFiles.Count > 0)
                        anyHbsFilesFound = true;
                }

                if (handlebarsFiles.Any())
                {
                    workspace = CompileHandlebarsFiles(project, workspace, handlebarsFiles, options);
                }
            }
            if (!anyHbsFilesFound)
                NoTemplates();
        }

        /// <summary>
        /// Checks if file needs to be compiled according to configured black or whitelisted directories
        /// </summary>
        /// <param name="file"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private static bool ShouldCompileFile(string file, CompilerOptions options)
        {
            if (options.DirectoryBlacklist.Any())
            {
                return (!options.DirectoryBlacklist.Any(x => file.StartsWith(x)));
            }
            else if (options.DirectoryWhitelist.Any())
            {
                return (options.DirectoryWhitelist.Any(x => file.StartsWith(x)));
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Calls the compiler for every handlebars-file provided.
        /// Tries to resolve dependencies (partial templates) by rerunning failed compilations
        /// </summary>
        /// <param name="project">Project containing the handlebars-files</param>	
        /// <param name="workspace">A Workspace instance where compiled templates can be inserted</param>
        /// <param name="hbsFiles">A List of handlebars-files to compile</param>
        /// <param name="options">The Compiler-Options as provided by user input</param>
        /// <returns></returns>
        private static Workspace CompileHandlebarsFiles(Project project, Workspace workspace, List<string> hbsFiles, CompilerOptions options)
        {
            bool successFullCompilation = true;
            // As long as there are files to compile and the previous run was successfull
            while (hbsFiles.Any() && successFullCompilation)
            {
                successFullCompilation = false;
                var nextRound = new List<string>();
                var partialErrors = new List<string>();
                foreach (var file in hbsFiles)
                {
                    //For each template
                    var fileInfo = new FileInfo(file);
                    string @namespace;

                    bool compiledVersionExists = File.Exists($"{file}.cs");
                    bool compiledVersionIsOlder = true;
                    if (compiledVersionExists)
                    {//Compiled Version already exists
                        var compiledFileInfo = new FileInfo($"{file}.cs");
                        compiledVersionIsOlder = (fileInfo.LastWriteTimeUtc > compiledFileInfo.LastWriteTimeUtc);
                        //Get the namespace from the csharp code
                        @namespace = DetermineNamespace(compiledFileInfo);
                    }
                    else
                    {
                        //No compiled version found.
                        //Get the namespace from Roslyn
                        @namespace = DetermineNamespace(fileInfo, project);
                    }
                    if (compiledVersionIsOlder || options.ForceRecompilation)
                    {// Try to compile if the template changed or if the user wants to
                        string content = File.ReadAllText(file);
                        string name = Path.GetFileNameWithoutExtension(file);
                        Tuple<string, IEnumerable<HandlebarsException>> compilationResult = CompileHandlebarsTemplate(content, @namespace, name, project, options);
                        if (!options.DryRun)
                        {
                            if (compilationResult?.Item2?.Any() ?? false)
                            {//Errors occured
                                _ExitCode = -1;
                                if (compilationResult.Item2.OfType<HandlebarsTypeError>().Any(x => x.Kind == HandlebarsTypeErrorKind.UnknownPartial || x.Kind == HandlebarsTypeErrorKind.UnknownLayout))
                                {//Unresolvable Partial... could be due to compiling sequence
                                    foreach (var error in compilationResult.Item2)
                                    {
                                        partialErrors.Add($"Compilation of '{name}' failed: {error.Message}");
                                    }
                                    nextRound.Add(file);
                                }
                                else
                                {
                                    foreach (var error in compilationResult.Item2)
                                        PrintError(error);
                                }
                            }
                            else
                            {
                                successFullCompilation = true;
                                //Check if template already exits
                                var doc = project.Documents.FirstOrDefault(x => x.Name.Equals(string.Concat(name, ".hbs.cs")));
                                if (doc != null)
                                {//And change it if it does
                                    project = doc.WithSyntaxRoot(CSharpSyntaxTree.ParseText(SourceText.From(compilationResult.Item1)).GetRoot()).Project;
                                    if (options.NetCoreProject)
                                    {
                                        //For .net core projects -> directly to the filesystem
                                        File.WriteAllText($"{file}.cs", compilationResult.Item1);
                                    }
                                }
                                else
                                {//Otherwise add a new document
                                    if (options.NetCoreProject)
                                    {
                                        //For .net core projects -> directly to the filesystem
                                        File.WriteAllText($"{file}.cs", compilationResult.Item1);
                                    }
                                    else
                                    {  //for .net projects through the .csproj
                                        project = project.AddDocument(string.Concat(name, ".hbs.cs"), SourceText.From(compilationResult.Item1), GetFolderStructureForFile(fileInfo, project)).Project;
                                    }
                                }
                                workspace.TryApplyChanges(project.Solution);
                                project = workspace.CurrentSolution.Projects.First(x => x.Id.Equals(project.Id));
                            }
                        }
                    }
                }
                hbsFiles = nextRound;
                if (!successFullCompilation)
                {
                    _ExitCode = -1;
                }
                if (partialErrors.Any() && !successFullCompilation)
                {
                    foreach (var err in partialErrors)
                    {
                        Console.Error.WriteLine(err);
                    }
                }
            }
            return workspace;
        }

        private static void PrintError(HandlebarsException error)
        {
            Console.Error.WriteLine($"*** Compilation failed: {error.Message}");
        }

        /// <summary>
        /// Calls the actual compiler. Returns the resulting code and a list of compiler errors
        /// </summary>
        /// <param name="content">The Template's Content</param>
        /// <param name="namespace">Its Namespace</param>
        /// <param name="name">Its Name</param>
        /// <param name="containingProject">Its Project</param>
        /// <param name="options">Compiler Options as provided by the user</param>
        /// <returns></returns>
        private static Tuple<string, IEnumerable<HandlebarsException>> CompileHandlebarsTemplate(string content, string @namespace, string name, Project containingProject, CompilerOptions options)
        {
            if (options.DryRun)
            {
                Console.WriteLine($"Compile file '{name}' in namespace '{@namespace}'");
                return null;
            }
            else
            {
                Console.WriteLine($"Compiling '{name}' ({@namespace})");
                return HbsCompiler.Compile(content, @namespace, name, containingProject);
            }
        }

        /// <summary>
        /// Get the Namespace for a template from its compiled counterpart.
        /// Do this by parsing the code (with Roslyn) and finding the namespacedeclaration
        /// </summary>
        /// <param name="compiledTemplate"></param>
        /// <returns></returns>
        private static string DetermineNamespace(FileInfo compiledTemplate)
        {
            var code = File.ReadAllText(compiledTemplate.FullName);
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            return root.Members.OfType<NamespaceDeclarationSyntax>().First().Name.ToString();
        }

        /// <summary>
        /// Get the Namespace for a template by asking the containing Project	
        /// </summary>
        /// <param name="hbsFile"></param>
        /// <param name="containingProject"></param>
        /// <returns></returns>
        private static string DetermineNamespace(FileInfo hbsFile, Project containingProject)
        {
            // Get 
            string templateDir = Path.GetDirectoryName(hbsFile.FullName);
            string projectDir = Path.GetDirectoryName(containingProject.FilePath);
            return string.Concat(containingProject.AssemblyName, templateDir.Substring(projectDir.Length).Replace(Path.DirectorySeparatorChar, '.'));
        }

        /// <summary>
        /// Returns a list of folders a file is in.		
        /// </summary>
        /// <param name="file"></param>
        /// <param name="containingProject"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetFolderStructureForFile(FileInfo file, Project containingProject)
        {
            string fileDir = Path.GetDirectoryName(file.FullName);
            string projectDir = Path.GetDirectoryName(containingProject.FilePath);
            return fileDir.Substring(projectDir.Length).Split(new char[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static void NoTemplates()
        {
            Console.Error.WriteLine("No handlebars templates found. Did you set build action to 'None' or 'C# analyzer additional file' (asp.net core for .net core)?");
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage: HandlebarsCompiler.exe -[flags] ([-e<Excluded Directory>]*|[-i<Included Directory>]*)  <SolutionFile>|<ProjectFile>");
            Console.WriteLine("Flags:");
            Console.WriteLine("      -n");
            Console.WriteLine("         Dry-Run: don't actually compile anything, just show what would be done ");
            Console.WriteLine("      -f");
            Console.WriteLine("         Force Compilation: compile handlebars-file even if it did not change");
            Console.WriteLine("      -c");
            Console.WriteLine("         .net core Project: The compiler will not add compiled files to its project file");
            Console.WriteLine("      -d");
            Console.WriteLine("         Print diagnostic messages from MsBuildWorkspace");
            Console.WriteLine("      -w");
            Console.WriteLine("         Watch the target project for changes");
            Console.WriteLine("");
            Console.WriteLine("Directory Black-     and Whitelists:");
            Console.WriteLine("-e<Exluded Directory> excludes a directory from compilation. Handlebars files in this folder will be ignored. Multiple statements possible.");
            Console.WriteLine("-i<Included Directory> includeds a directory from compilation. Only Handlebars files in this folder will be compiled. Multiple statements possible.");
        }

        private static void FileDoesNotExist(string file)
        {
            Console.WriteLine($"File '{file}' does not exist!");
        }
    }
}
