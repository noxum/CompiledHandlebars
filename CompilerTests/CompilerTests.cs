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
using CompiledHandlebars.CompilerTests.TestViewModels;

namespace CompiledHandlebars.Compiler.Tests
{

  /// <summary>
  /// TODO: Read and understand https://benetkiewicz.github.io/blog/csharp/roslyn/2014/10/26/code-mutiations-with-roslyn.html
  /// Should be helpful for unittesting generated code
  /// </summary>
  [TestClass()]
  public class CompilerTests
  {
    private const string _marsModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.MarsModel}}";
    private Assembly assemblyWithCompiledTemplates { get; set; }

    [TestInitialize()]
    public void Initialize()
    {
      assemblyWithCompiledTemplates = CompilerHelper.CompileTemplatesToAssembly(this.GetType());

    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("BasicTest", "{{Name}}", _marsModel)]
    public void BasicTest()
    {
      Assert.IsTrue(ShouldRender("BasicTest", MarsModelFactory.CreateFullMarsModel(), "Mars"));  
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("IfTest", @"{{#if Name}}HasName{{else}}HasNoName{{/if}}", _marsModel)]
    public void IfTest()
    {
      Assert.IsTrue(ShouldRender("IfTest", MarsModelFactory.CreateFullMarsModel(), "HasName"));
      Assert.IsTrue(ShouldRender("IfTest", new MarsModel(), "HasNoName"));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("UnlessTest", @"{{#unless Name}}HasNoName{{else}}HasName{{/unless}}", _marsModel)]
    public void UnlessTest()
    {
      Assert.IsTrue(ShouldRender("UnlessTest", MarsModelFactory.CreateFullMarsModel(), "HasName"));
      Assert.IsTrue(ShouldRender("UnlessTest", new MarsModel(), "HasNoName"));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("WithTest", @"{{#with Phobos}}Name:{{Name}}{{/with}}", _marsModel)]
    public void WithTest()
    {
      Assert.IsTrue(ShouldRender("WithTest", MarsModelFactory.CreateFullMarsModel(), "Name:Phobos"));
      Assert.IsTrue(ShouldRender("WithTest", new MarsModel(), ""));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("PathTest1", @"{{Phobos/../Name}}:{{Pobos.Name}}", _marsModel)]
    [RegisterHandlebarsTemplate("PathTest2", @"{{Deimos/../Name}}:{{Deimos/../Phobos.Name}}", _marsModel)]
    public void PathTest()
    {
      Assert.IsTrue(ShouldRender("PathTest1", MarsModelFactory.CreateFullMarsModel(), "Mars:Phobos"));
      Assert.IsTrue(ShouldRender("PathTest2", MarsModelFactory.CreateFullMarsModel(), "Mars:Phobos"));
    }

    private bool ShouldRender<TViewModel>(string templateName, TViewModel viewModel, string result)
    {
      var template = assemblyWithCompiledTemplates.GetType($"TestTemplates.{templateName}");
      var renderResult = template.GetMethod("Render").Invoke(null, new object[] { viewModel }) as string;
      return result.Equals(renderResult);
    }
  }
}