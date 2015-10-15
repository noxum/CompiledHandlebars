using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompiledHandlebars.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.CompilerTests.Helper;
using Microsoft.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace CompiledHandlebars.Compiler.Tests
{

  public class MarsModel
  {
    public string Name { get; set; }
  }

  /// <summary>
  /// TODO: Read and understand https://benetkiewicz.github.io/blog/csharp/roslyn/2014/10/26/code-mutiations-with-roslyn.html
  /// Should be helpful for unittesting generated code
  /// </summary>
  [TestClass()]
  public class CompilerTests
  {

    private Assembly assemblyWithCompiledTemplates { get; set; }

    [TestInitialize()]
    public void Initialize()
    {
      var solutionFile = Path.Combine(Directory.CreateDirectory(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, "CompiledHandlebars.sln");
      List<SyntaxTree> compiledTemplates = new List<SyntaxTree>();  
      var workspace = MSBuildWorkspace.Create();
      var sol = workspace.OpenSolutionAsync(solutionFile).Result;
      foreach (MethodInfo methodInfo in (this.GetType()).GetMethods())
      {
        var attrList = methodInfo.GetCustomAttributes(typeof(RegisterHandlebarsTemplateAttribute), false) as RegisterHandlebarsTemplateAttribute[];
        foreach(var template in attrList)
        {//Get compiled templates
          var code = HbsCompiler.Compile(template._contents, "TestTemplates", template._name, workspace).Item1;
          compiledTemplates.Add(CSharpSyntaxTree.ParseText(code));
        }        
      }

      string assemblyName = Path.GetRandomFileName();
      MetadataReference[] references = new MetadataReference[]
      {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(WebUtility).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(CompilerTests).Assembly.Location),
      };
      var compilation = CSharpCompilation.Create(
        assemblyName,
        compiledTemplates,
        references,
        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      using (var ms = new MemoryStream())
      {
        var emitResult = compilation.Emit(ms);

        if(!emitResult.Success)
        {
          IEnumerable<Diagnostic> failures = emitResult.Diagnostics.Where(diagnostic =>
                          diagnostic.IsWarningAsError ||
                          diagnostic.Severity == DiagnosticSeverity.Error);
          throw new Exception(failures.First().GetMessage());
        }
        ms.Seek(0, SeekOrigin.Begin);
        assemblyWithCompiledTemplates = Assembly.Load(ms.ToArray());
      }

    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("Mars", @"{{model CompiledHandlebars.Compiler.Tests.MarsModel}}{{Name}}")]
    public void CompileTest()
    {
      Assert.IsTrue(ShouldRender("Mars", new MarsModel() { Name = "Mars" }, "Mars"));  
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("MarsConditionals", @"{{model CompiledHandlebars.Compiler.Tests.MarsModel}}{{#if Name}}HasName{{else}}HasNoName{{/if}}")]
    public void ConditionalsTest()
    {
      Assert.IsTrue(ShouldRender("MarsConditionals", new MarsModel() { Name = "Mars"}, "HasName"));
      Assert.IsTrue(ShouldRender("MarsConditionals", new MarsModel(), "HasNoName"));
    }

    private bool ShouldRender<TViewModel>(string templateName, TViewModel viewModel, string result)
    {
      var template = assemblyWithCompiledTemplates.GetType($"TestTemplates.{templateName}");
      var renderResult = template.GetMethod("Render").Invoke(null, new object[] { viewModel }) as string;
      return result.Equals(renderResult);
    }
  }
}