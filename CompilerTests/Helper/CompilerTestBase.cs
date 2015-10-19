using CompiledHandlebars.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.Helper
{
  public abstract class CompilerTestBase
  {
    protected static Dictionary<string, Tuple<string, IEnumerable<HandlebarsException>>> compiledCode { get; set; }  = new Dictionary<string, Tuple<string, IEnumerable<HandlebarsException>>>();
    protected static Assembly assemblyWithCompiledTemplates { get; set; }

    protected static Assembly CompileTemplatesToAssembly(Type testClassType)
    {
      var solutionFile = Path.Combine(Directory.CreateDirectory(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, "CompiledHandlebars.sln");
      List<SyntaxTree> compiledTemplates = new List<SyntaxTree>();
      var workspace = MSBuildWorkspace.Create();
      var adHocWorkspace = new AdhocWorkspace();
      var sol = workspace.OpenSolutionAsync(solutionFile).Result;
      var project = sol.Projects.First(x => x.Name.Equals("CompiledHandlebars.CompilerTests"));

      foreach (MethodInfo methodInfo in (testClassType).GetMethods())
      {
        var attrList = methodInfo.GetCustomAttributes(typeof(RegisterHandlebarsTemplateAttribute), false) as RegisterHandlebarsTemplateAttribute[];
        foreach (var template in attrList)
        {//Get compiled templates
          var code = HbsCompiler.Compile(template._contents, "TestTemplates", template._name, workspace);
          compiledCode.Add(template._name, code);
          if (template._include)
          {
            //Check if template already exits
            var doc = project.Documents.FirstOrDefault(x => x.Name.Equals(string.Concat(template._name, ".cs")));
            if (doc != null)
            {//And change it if it does
              project = doc.WithSyntaxRoot(CSharpSyntaxTree.ParseText(code.Item1).GetRoot()).Project;
            } else
            {//Otherwise add a new document
              project = project.AddDocument(string.Concat(template._name, ".cs"), SourceText.From(code.Item1), new string[] { "TestTemplates", testClassType.Name }).Project;
            }
            //Then add the new version
            compiledTemplates.Add(CSharpSyntaxTree.ParseText(code.Item1));
            workspace.TryApplyChanges(project.Solution);
            project = workspace.CurrentSolution.Projects.First(x => x.Name.Equals("CompiledHandlebars.CompilerTests"));
          }
        }
      }
      string assemblyName = Path.GetRandomFileName();
      MetadataReference[] references = new MetadataReference[]
      {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(WebUtility).Assembly.Location),
        MetadataReference.CreateFromFile(testClassType.Assembly.Location),
      };
      var compilation = CSharpCompilation.Create(
        assemblyName,
        compiledTemplates,
        references,
        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      using (var ms = new MemoryStream())
      {
        var emitResult = compilation.Emit(ms);

        if (!emitResult.Success)
        {
          IEnumerable<Diagnostic> failures = emitResult.Diagnostics.Where(diagnostic =>
                          diagnostic.IsWarningAsError ||
                          diagnostic.Severity == DiagnosticSeverity.Error);

          /*foreach (var fail in failures)
          {
            project.Documents.First(x => x.FilePath.Equals(fail.Location.SourceTree.FilePath));
                       
          }*/
          throw new Exception(failures.First().GetMessage());
        }
        ms.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(ms.ToArray());
      }
    }

    protected void ShouldRender<TViewModel>(string templateName, TViewModel viewModel, string expectedResult)
    {
      var template = assemblyWithCompiledTemplates.GetType($"TestTemplates.{templateName}");
      var renderResult = template.GetMethod("Render").Invoke(null, new object[] { viewModel }) as string;
      Assert.AreEqual(expectedResult, renderResult);
    }

    protected void ShouldContainCode(string templateName, string code, int instances)
    {
      Assert.IsTrue(Regex.Matches(compiledCode[templateName].Item1, code).Count == instances);
    }

    protected void ShouldRaiseError(string templateName, HandlebarsSyntaxErrorKind kind)
    {
      Assert.IsTrue(compiledCode[templateName].Item2.OfType<HandlebarsSyntaxError>().Any(x => x.Kind.Equals(kind)));
    }

    protected void ShouldRaiseError(string templateName, HandlebarsTypeErrorKind kind)
    {
      Assert.IsTrue(compiledCode[templateName].Item2.OfType<HandlebarsTypeError>().Any(x => x.Kind.Equals(kind)));
    }

    protected void ShouldCompileWithoutError(string templateName)
    {
      Assert.IsTrue(!compiledCode[templateName].Item2.Any());
    }
  }
}
