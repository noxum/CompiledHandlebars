using CompiledHandlebars.CompilerTests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins
{
  public class GoodbyeModel
  {
    public string StrGoodbye { get; set; }
    public bool BoolGoodbye { get; set; }
    public int IntGoodbye { get; set; }
    public List<string> ListGoodbye { get; set; }
    public string World { get; set; }
  }


  /// <summary>
  /// https://github.com/wycats/handlebars.js/blob/master/spec/builtins.js
  /// </summary>
  [TestClass]
  public class BuiltinsTest : CompilerTestBase
  {



    private const string _goodbyeModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.GoodbyeModel}}";

    static BuiltinsTest()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(BuiltinsTest));
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("If1", "{{#if StrGoodbye}}GOODBYE {{/if}}cruel {{World}}!", _goodbyeModel)]
    [RegisterHandlebarsTemplate("If2", "{{#if IntGoodbye}}GOODBYE {{/if}}cruel {{World}}!", _goodbyeModel)]
    [RegisterHandlebarsTemplate("If3", "{{#if BoolGoodbye}}GOODBYE {{/if}}cruel {{World}}!", _goodbyeModel)]
    [RegisterHandlebarsTemplate("If4", "{{#if ListGoodbye}}GOODBYE {{/if}}cruel {{World}}!", _goodbyeModel)]
    public void If()
    {
      ShouldRender("If3", new GoodbyeModel() { BoolGoodbye = true, World = "world" }, "GOODBYE cruel world!");
      ShouldRender("If1", new GoodbyeModel() { StrGoodbye = "dummy", World = "world" }, "GOODBYE cruel world!");
      ShouldRender("If3", new GoodbyeModel() { BoolGoodbye = false, World = "world" }, "cruel world!");
      ShouldRender("If3", new GoodbyeModel() { World = "world" }, "cruel world!");
      ShouldRender("If4", new GoodbyeModel() { ListGoodbye = new List<string>() { "foo" }, World = "world" }, "GOODBYE cruel world!");
      ShouldRender("If4", new GoodbyeModel() { ListGoodbye = new List<string>() { }, World = "world" }, "cruel world!");
      ShouldRender("If2", new GoodbyeModel() { IntGoodbye = 0, World = "world" }, "cruel world!");
    }

  }
}
