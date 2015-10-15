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

  [TestClass()]
  public class CompilerTests : CompilerTestBase
  {
    private const string _marsModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.MarsModel}}";
    
    static CompilerTests()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(CompilerTests));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("BasicTest", "{{Name}}", _marsModel)]
    public void BasicTest()
    {
      Assert.IsTrue(ShouldRender("BasicTest", MarsModelFactory.CreateFullMarsModel(), "Mars"));  
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("HtmlEncodeTest1", "{{Description}}", _marsModel)]
    [RegisterHandlebarsTemplate("HtmlEncodeTest2", "{{{Description}}}", _marsModel)]

    public void HtmlEncodeTest()
    {
      Assert.IsTrue(ShouldRender("HtmlEncodeTest1", MarsModelFactory.CreateFullMarsModel(), @"&lt;b&gt;Mars&lt;/b&gt; is the fourth &lt;a href=&quot;/wiki/Planet&quot; title=&quot;Planet&quot;&gt;planet&lt;/a&gt; from the &lt;a href=&quot;/wiki/Sun&quot; title=&quot;Sun&quot;&gt;Sun&lt;/a&gt; and the second smallest planet in the &lt;a href=&quot;/wiki/Solar_System&quot; title=&quot;Solar System&quot;&gt;Solar System&lt;/a&gt;, after &lt;a href=&quot;/wiki/Mercury_(planet)&quot; title=&quot;Mercury (planet)&quot;&gt;Mercury&lt;/a&gt;."));
      Assert.IsTrue(ShouldRender("HtmlEncodeTest2", MarsModelFactory.CreateFullMarsModel(), "<b>Mars</b> is the fourth <a href=\"/wiki/Planet\" title=\"Planet\">planet</a> from the <a href=\"/wiki/Sun\" title=\"Sun\">Sun</a> and the second smallest planet in the <a href=\"/wiki/Solar_System\" title=\"Solar System\">Solar System</a>, after <a href=\"/wiki/Mercury_(planet)\" title=\"Mercury (planet)\">Mercury</a>."));          
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("IfTest", @"{{#if Name}}HasName{{/if}}", _marsModel)]
    public void IfTest()
    {
      Assert.IsTrue(ShouldRender("IfTest", MarsModelFactory.CreateFullMarsModel(), "HasName"));
      Assert.IsTrue(ShouldRender("IfTest", new MarsModel(), ""));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("IfElseTest", @"{{#if Name}}HasName{{else}}HasNoName{{/if}}", _marsModel)]
    public void IfElseTest()
    {
      Assert.IsTrue(ShouldRender("IfElseTest", MarsModelFactory.CreateFullMarsModel(), "HasName"));
      Assert.IsTrue(ShouldRender("IfElseTest", new MarsModel(), "HasNoName"));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("NestedIfTest", @"{{#if Phobos}}Phobos:{{#if Phobos.Name}}HasName{{else}}HasNoName{{/if}}{{else}}NoPhobos{{/if}}", _marsModel)]
    public void NestedIfTest()
    {
      var mars = MarsModelFactory.CreateFullMarsModel();
      Assert.IsTrue(ShouldRender("NestedIfTest", mars, "Phobos:HasName"));
      mars.Phobos.Name = null;
      Assert.IsTrue(ShouldRender("NestedIfTest", mars, "Phobos:HasNoName"));
      Assert.IsTrue(ShouldRender("NestedIfTest", new MarsModel(), "NoPhobos"));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("UnlessElseTest", @"{{#unless Name}}HasNoName{{else}}HasName{{/unless}}", _marsModel)]
    public void UnlessElseTest()
    {
      Assert.IsTrue(ShouldRender("UnlessElseTest", MarsModelFactory.CreateFullMarsModel(), "HasName"));
      Assert.IsTrue(ShouldRender("UnlessElseTest", new MarsModel(), "HasNoName"));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("UnlessTest", @"{{#unless Name}}HasNoName{{/unless}}", _marsModel)]
    public void UnlessTest()
    {
      Assert.IsTrue(ShouldRender("UnlessTest", MarsModelFactory.CreateFullMarsModel(), ""));
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
    [RegisterHandlebarsTemplate("PathTest1", @"{{Phobos/../Name}}:{{Phobos.Name}}", _marsModel)]
    [RegisterHandlebarsTemplate("PathTest2", @"{{Deimos/../Name}}:{{Deimos/../Phobos.Name}}", _marsModel)]
    public void PathTest()
    {
      Assert.IsTrue(ShouldRender("PathTest1", MarsModelFactory.CreateFullMarsModel(), "Mars:Phobos"));
      Assert.IsTrue(ShouldRender("PathTest2", MarsModelFactory.CreateFullMarsModel(), "Mars:Phobos"));
    }

  }
}