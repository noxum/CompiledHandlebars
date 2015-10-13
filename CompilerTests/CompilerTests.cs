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

namespace CompiledHandlebars.Compiler.Tests
{

  [TestViewModel]
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
      var newSol = sol;
      var project = newSol.Projects.First(x => x.Name.Equals("CompiledHandlebars.CompilerTests"));
      foreach (MethodInfo methodInfo in (this.GetType()).GetMethods())
      {
        var attrList = methodInfo.GetCustomAttributes(typeof(RegisterHandlebarsTemplateAttribute), false) as RegisterHandlebarsTemplateAttribute[];
        foreach(var template in attrList)
        {//Get compiled templates
          var code = HbsCompiler.Compile(template._contents, "TestTemplates", template._name, workspace).Item1;
          project = project.AddDocument(string.Concat(template._name, ".cs"), SourceText.From(code), new string[]{ "TestTemplates"}).Project;
        }        
      }
      newSol = project.Solution;
      workspace.TryApplyChanges(newSol);
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("Mars", @"{{model CompiledHandlebars.Compiler.Tests.MarsModel}}{{Name}}")]
    public void CompileTest()
    {
      var result = TestTemplates.Mars.Render(new MarsModel() { Name = "Mars" } );
      Assert.AreEqual("Mars", result);      
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("MarsConditionals", @"{{model CompiledHandlebars.Compiler.Tests.MarsModel}}{{#if Name}}HasName{{else}}HasNoName{{/if}}")]
    public void ConditionalsTest()
    {
   //   var result = TestTemplates.MarsConditionals.Render(new MarsModel() { Name = "Mars" });
   //   Assert.AreEqual("Mars", result);
    }
  }
}