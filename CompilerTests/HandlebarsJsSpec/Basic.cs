using CompiledHandlebars.CompilerTests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


/// <summary>
/// Tries to adopt the UnitTests from the original repository: https://github.com/wycats/handlebars.js/blob/master/spec/
/// </summary>
namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec
{

  public class FooModel
  {
    public string Foo { get; set; }
  }

  //These JavaScript guys... are they all depressed?
  public class GoodbyeCruelWorldModel
  {
    public string Goodbye { get; set; }
    public string Cruel { get; set; }
    public string World { get; set; }
  }


  public class NumModel1
  {
    public int Num1 { get; set; }
    public int Num2 { get; set; }
  }

  public class NumModel2
  {
    public NumModel1 Num1 { get; set; }
  }

  /// <summary>
  /// https://github.com/wycats/handlebars.js/blob/master/spec/basic.js
  /// </summary>
  [TestClass]
  public class BasicTests : CompilerTestBase
  {
    private const string _fooModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.FooModel}}";
    private const string _goodbyeCruelWorldModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.GoodbyeCruelWorldModel}}";
    private const string _numModel1 = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.NumModel1}}";
    private const string _numModel2 = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.NumModel2}}";

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

    [TestMethod]
    [RegisterHandlebarsTemplate("CompilingWithBasicContext1", "Goodbye\n{{Cruel}}\n{{World}}!", _goodbyeCruelWorldModel)]
    public void CompilingWithBasicContext()
    {
      ShouldRender("CompilingWithBasicContext1", new GoodbyeCruelWorldModel() { Cruel = "cruel", World = "world" }, "Goodbye\ncruel\nworld!");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("CompilingWithStringContext1", "{{model System.String}}{{.}}{{Length}}")]
    public void CompilingWithStringContext()
    {
      ShouldRender("CompilingWithStringContext1", "bye", "bye3");
    }

    //These tests do not quite fit. Both templates throw compile-time errors 
    [TestMethod]
    [RegisterHandlebarsTemplate("CompileWithUndefinedContext1", "Goodbye\n{{cruel}}\n{{world.bar}}!", _fooModel)]
    [RegisterHandlebarsTemplate("CompileWithUndefinedContext2", "{{#unless Foo}}Goodbye{{../test}}{{test2}}{{/unless}}", _fooModel)]
    public void CompileWithUndefinedContext()
    {
      ShouldRender("CompileWithUndefinedContext1", default(FooModel), "Goodbye\n\n!");
      ShouldRender("CompileWithUndefinedContext2", default(FooModel), "Goodbye");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("Comments1", "{{! Goodbye}}Goodbye\n{{Cruel}}\n{{World}}!", _goodbyeCruelWorldModel)]
    [RegisterHandlebarsTemplate("Comments2", "    {{~! comment ~}}      blah", _fooModel)]
    [RegisterHandlebarsTemplate("Comments3", "    {{~!-- long-comment --~}}      blah", _fooModel)]
    [RegisterHandlebarsTemplate("Comments4", "    {{! comment ~}}      blah", _fooModel)]
    [RegisterHandlebarsTemplate("Comments5", "    {{!-- long-comment --~}}      blah", _fooModel)]
    [RegisterHandlebarsTemplate("Comments6", "    {{~! comment}}      blah", _fooModel)]
    [RegisterHandlebarsTemplate("Comments7", "    {{~!-- long-comment --}}      blah", _fooModel)]
    public void Comments()
    {
      ShouldRender("Comments1", new GoodbyeCruelWorldModel() { Cruel = "cruel", World = "world" }, "Goodbye\ncruel\nworld!");
      ShouldRender("Comments2", default(FooModel), "blah");
      ShouldRender("Comments3", default(FooModel), "blah");
      ShouldRender("Comments4", default(FooModel), "    blah");
      ShouldRender("Comments5", default(FooModel), "    blah");
      ShouldRender("Comments6", default(FooModel), "      blah");
      ShouldRender("Comments7", default(FooModel), "      blah");
    }

    /* Mustache blocks not supported (yet?!)
    it('boolean', function() {
    var string = '{{#goodbye}}GOODBYE {{/goodbye}}cruel {{world}}!';
    shouldCompileTo(string, {goodbye: true, world: 'world'}, 'GOODBYE cruel world!',
                    'booleans show the contents when true');

    shouldCompileTo(string, {goodbye: false, world: 'world'}, 'cruel world!',
                    'booleans do not show the contents when false');
  });*/

    [TestMethod]
    [RegisterHandlebarsTemplate("Zeros1", "num1: {{Num1}}, num2: {{Num2}}", _numModel1)]
    [RegisterHandlebarsTemplate("Zeros2", "{{model System.Int32}}num: {{.}}")]
    [RegisterHandlebarsTemplate("Zeros3", "num: {{Num1/Num2}}", _numModel2)]
    public void Zeros()
    {
      ShouldRender("Zeros1", new NumModel1() { Num1 = 42, Num2 = 0 }, "num1: 42, num2: 0");
      ShouldRender("Zeros2", 0, "num: 0");
      ShouldRender("Zeros3", new NumModel2() { Num1 = new NumModel1() { Num2 = 0 } }, "num: 0");
    }

  }
}
