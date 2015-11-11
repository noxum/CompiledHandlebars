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

namespace CompiledHandlebars.CompilerTests
{

  [TestClass()]
  public class BasicCompilerTests : CompilerTestBase
  {
    private const string _marsModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.MarsModel}}";
    private const string _starModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.StarModel}}";

    static BasicCompilerTests()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(BasicCompilerTests));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("BasicTest", "{{Name}}", _marsModel)]
    public void BasicTest()
    {
      ShouldRender("BasicTest", MarsModelFactory.CreateFullMarsModel(), "Mars");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("HtmlEncodeTest1", "{{Description}}", _marsModel)]
    [RegisterHandlebarsTemplate("HtmlEncodeTest2", "{{{Description}}}", _marsModel)]
    public void HtmlEncodeTest()
    {
      ShouldRender("HtmlEncodeTest1", MarsModelFactory.CreateFullMarsModel(), @"&lt;b&gt;Mars&lt;/b&gt; is the fourth &lt;a href=&quot;/wiki/Planet&quot; title=&quot;Planet&quot;&gt;planet&lt;/a&gt; from the &lt;a href=&quot;/wiki/Sun&quot; title=&quot;Sun&quot;&gt;Sun&lt;/a&gt; and the second smallest planet in the &lt;a href=&quot;/wiki/Solar_System&quot; title=&quot;Solar System&quot;&gt;Solar System&lt;/a&gt;, after &lt;a href=&quot;/wiki/Mercury_(planet)&quot; title=&quot;Mercury (planet)&quot;&gt;Mercury&lt;/a&gt;.");
      ShouldRender("HtmlEncodeTest2", MarsModelFactory.CreateFullMarsModel(), "<b>Mars</b> is the fourth <a href=\"/wiki/Planet\" title=\"Planet\">planet</a> from the <a href=\"/wiki/Sun\" title=\"Sun\">Sun</a> and the second smallest planet in the <a href=\"/wiki/Solar_System\" title=\"Solar System\">Solar System</a>, after <a href=\"/wiki/Mercury_(planet)\" title=\"Mercury (planet)\">Mercury</a>.");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("IfTest", @"{{#if Name}}HasName{{/if}}", _marsModel)]
    public void IfTest()
    {
      ShouldRender("IfTest", MarsModelFactory.CreateFullMarsModel(), "HasName");
      ShouldRender("IfTest", new MarsModel(), "");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("IfElseTest", @"{{#if Name}}HasName{{else}}HasNoName{{/if}}", _marsModel)]
    public void IfElseTest()
    {
      ShouldRender("IfElseTest", MarsModelFactory.CreateFullMarsModel(), "HasName");
      ShouldRender("IfElseTest", new MarsModel(), "HasNoName");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("NestedIfTest", @"{{#if Phobos}}Phobos:{{#if Phobos.Name}}HasName{{else}}HasNoName{{/if}}{{else}}NoPhobos{{/if}}", _marsModel)]
    public void NestedIfTest()
    {
      var mars = MarsModelFactory.CreateFullMarsModel();
      ShouldRender("NestedIfTest", mars, "Phobos:HasName");
      mars.Phobos.Name = null;
      ShouldRender("NestedIfTest", mars, "Phobos:HasNoName");
      ShouldRender("NestedIfTest", new MarsModel(), "NoPhobos");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("UnlessElseTest", @"{{#unless Name}}HasNoName{{else}}HasName{{/unless}}", _marsModel)]
    public void UnlessElseTest()
    {
      ShouldRender("UnlessElseTest", MarsModelFactory.CreateFullMarsModel(), "HasName");
      ShouldRender("UnlessElseTest", new MarsModel(), "HasNoName");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("UnlessTest", @"{{#unless Name}}HasNoName{{/unless}}", _marsModel)]
    public void UnlessTest()
    {
      ShouldRender("UnlessTest", MarsModelFactory.CreateFullMarsModel(), "");
      ShouldRender("UnlessTest", new MarsModel(), "HasNoName");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("WithTest", @"{{#with Phobos}}Name:{{Name}}{{/with}}", _marsModel)]
    public void WithTest()
    {
      ShouldRender("WithTest", MarsModelFactory.CreateFullMarsModel(), "Name:Phobos");
      ShouldRender("WithTest", new MarsModel(), "");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("PathTest1", @"{{Phobos/../Name}}:{{Phobos.Name}}", _marsModel)]
    [RegisterHandlebarsTemplate("PathTest2", @"{{Deimos/../Name}}:{{Deimos/../Phobos.Name}}", _marsModel)]
    public void PathTest()
    {
      ShouldRender("PathTest1", MarsModelFactory.CreateFullMarsModel(), "Mars:Phobos");
      ShouldRender("PathTest2", MarsModelFactory.CreateFullMarsModel(), "Mars:Phobos");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("ThisTest1", @"{{model System.String}}{{this}}")]
    [RegisterHandlebarsTemplate("ThisTest2", @"{{#with Name}}{{this}}{{/with}}", _marsModel)]
    [RegisterHandlebarsTemplate("ThisTest3", @"{{this.Name}}", _marsModel)]
    [RegisterHandlebarsTemplate("ThisTest4", @"{{#if this}}Model{{else}}NoModel{{/if}}", _marsModel)]
    [RegisterHandlebarsTemplate("ThisTest5", @"{{./Name}}", _marsModel)]
    [RegisterHandlebarsTemplate("ThisTest6", @"{{#with Name}}{{.}}{{/with}}", _marsModel)]
    public void ThisTest()
    {
      ShouldRender("ThisTest1", "Test", "Test");
      ShouldRender("ThisTest2", MarsModelFactory.CreateFullMarsModel(), "Mars");
      ShouldRender("ThisTest3", MarsModelFactory.CreateFullMarsModel(), "Mars");
      ShouldRender("ThisTest4", MarsModelFactory.CreateFullMarsModel(), "Model");
      ShouldRender("ThisTest4", default(MarsModel), "NoModel");
      ShouldRender("ThisTest5", MarsModelFactory.CreateFullMarsModel(), "Mars");
      ShouldRender("ThisTest6", MarsModelFactory.CreateFullMarsModel(), "Mars");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("EachTest1", @"{{#each Plains}}{{Name}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("EachTest2", @"{{#each Mountains}}{{Name}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("EachTest3", @"{{#each Rovers}}{{Key}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("EachTest4", @"{{#each Planets}}{{Name}}:{{#each Moons}}{{Name}}{{/each}};{{/each}}", _starModel)]
    [RegisterHandlebarsTemplate("EachTest5", @"{{#with Planets}}{{#each this}}{{Name}};{{/each}}{{/with}}", _starModel)]
    public void EachTest()
    {
      ShouldRender("EachTest1", MarsModelFactory.CreateFullMarsModel(), "Acidalia PlanitiaUtopia Planitia");
      ShouldRender("EachTest2", MarsModelFactory.CreateFullMarsModel(), "Aeolis MonsOlympus Mons");
      ShouldRender("EachTest3", MarsModelFactory.CreateFullMarsModel(), "OpportunityCuriosity");
      ShouldRender("EachTest4", CelestialBodyFactory.CreateSolarSystem(), "Mercury:;Venus:;Earth:Moon;Mars:DeimosPhobos;");
      ShouldRender("EachTest5", CelestialBodyFactory.CreateSolarSystem(), "Mercury;Venus;Earth;Mars;");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("EmptyListsAreFalsyTest1", @"{{#if Mountains}}Mountains{{else}}no Mountains{{/if}}", _marsModel)]
    public void EmptyListsAreFalsyTest()
    {
      ShouldRender("EmptyListsAreFalsyTest1", MarsModelFactory.CreateFullMarsModel(), "Mountains");
      ShouldRender("EmptyListsAreFalsyTest1", new MarsModel(), "no Mountains");
      ShouldRender("EmptyListsAreFalsyTest1", new MarsModel() { Mountains = new List<MarsModel.Mountain>() } , "no Mountains");
    }
    

    [TestMethod]
    [RegisterHandlebarsTemplate("WhitespaceControlTest1", @"  {{~Name~}}  ", _marsModel)]
    [RegisterHandlebarsTemplate("WhitespaceControlTest2", "{{#each Plains~}}\n{{Name}}\n{{~/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("WhitespaceControlTest3", @"  {{{~Name~}}}  ", _marsModel)]
    public void WhitespaceControlTest()
    {
      ShouldRender("WhitespaceControlTest1", MarsModelFactory.CreateFullMarsModel(), "Mars");
      ShouldRender("WhitespaceControlTest2", MarsModelFactory.CreateFullMarsModel(), "Acidalia PlanitiaUtopia Planitia");
      ShouldRender("WhitespaceControlTest3", MarsModelFactory.CreateFullMarsModel(), "Mars");
    }


    [TestMethod]
    [RegisterHandlebarsTemplate("CommentTest1","{{!Name}}", _marsModel)]
    [RegisterHandlebarsTemplate("CommentTest2", "{{!--{{Name}}--}}", _marsModel)]
    [RegisterHandlebarsTemplate("CommentTest3", "No {{!Comment}}Comment", _marsModel)]
    public void CommentTest()
    {
      ShouldRender("CommentTest1", MarsModelFactory.CreateFullMarsModel(), "");
      ShouldRender("CommentTest2", MarsModelFactory.CreateFullMarsModel(), "");
      ShouldRender("CommentTest3", MarsModelFactory.CreateFullMarsModel(), "No Comment");
    }


  }
}