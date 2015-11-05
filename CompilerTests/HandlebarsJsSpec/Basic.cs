using CompiledHandlebars.CompilerTests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec
{

  public class FooModel
  {
    public string Foo { get; set; }
  }
  
  [TestClass]
  public class BasicTests : CompilerTestBase
  {
    private const string _fooModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.FooModel}}";
    static BasicTests()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(BasicTests));
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("MostBasic1", "{{Foo}}", _fooModel)]
    public void MostBasic()
    {
      ShouldRender("MostBasic1", new FooModel() { Foo = "foo" }, "foo");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("Escaping1", "\\{{Foo}}", _fooModel)]
    [RegisterHandlebarsTemplate("Escaping2", "content \\{{Foo}}", _fooModel)]
    [RegisterHandlebarsTemplate("Escaping3", "\\\\{{Foo}}", _fooModel)]
    [RegisterHandlebarsTemplate("Escaping4", "content \\\\{{Foo}}", _fooModel)]
    [RegisterHandlebarsTemplate("Escaping5", "\\\\ {{Foo}}", _fooModel)]
    public void Escaping()
    {
      ShouldRender("Escaping1", new FooModel() { Foo = "food" }, "{{Foo}}");
      ShouldRender("Escaping2", new FooModel() { Foo = "food" }, "content {{Foo}}");
      ShouldRender("Escaping3", new FooModel() { Foo = "food" }, "\\food");
      ShouldRender("Escaping4", new FooModel() { Foo = "food" }, "content \\food");
      ShouldRender("Escaping5", new FooModel() { Foo = "food" }, "\\\\ food");
    }

  }
}
