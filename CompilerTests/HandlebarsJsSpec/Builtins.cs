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
    public GoodbyeModel Foo { get; set; }
    public string StrGoodbye { get; set; }
    public bool BoolGoodbye { get; set; }
    public int IntGoodbye { get; set; }
    public List<string> ListGoodbye { get; set; }
    public string World { get; set; }
  }  

  public class PersonModel
  {
    public PersonNameModel Person { get; set; }
  }

  public class PersonNameModel
  {
    public string First { get; set; }
    public string Last { get; set; }
  }


  /// <summary>
  /// https://github.com/wycats/handlebars.js/blob/master/spec/builtins.js
  /// </summary>
  [TestClass]
  public class BuiltinsTest : CompilerTestBase
  {
    private const string _personModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.PersonModel}}";
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
    [RegisterHandlebarsTemplate("If5", "{{#with Foo}}{{#if BoolGoodbye}}GOODBYE cruel {{../World}}!{{/if}}{{/with}}", _goodbyeModel)]
    public void If()
    {
      ShouldRender("If3", new GoodbyeModel() { BoolGoodbye = true, World = "world" }, "GOODBYE cruel world!");
      ShouldRender("If1", new GoodbyeModel() { StrGoodbye = "dummy", World = "world" }, "GOODBYE cruel world!");
      ShouldRender("If3", new GoodbyeModel() { BoolGoodbye = false, World = "world" }, "cruel world!");
      ShouldRender("If3", new GoodbyeModel() { World = "world" }, "cruel world!");
      ShouldRender("If4", new GoodbyeModel() { ListGoodbye = new List<string>() { "foo" }, World = "world" }, "GOODBYE cruel world!");
      ShouldRender("If4", new GoodbyeModel() { ListGoodbye = new List<string>() { }, World = "world" }, "cruel world!");
      ShouldRender("If2", new GoodbyeModel() { IntGoodbye = 0, World = "world" }, "cruel world!");
      ShouldRender("If5", new GoodbyeModel() { World = "world", Foo = new GoodbyeModel() { BoolGoodbye = true } }, "GOODBYE cruel world!");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("With1", "{{#with Person}}{{First}} {{Last}}{{/with}}", _personModel)]    
    public void With()
    {
      ShouldRender("With1", new PersonModel() { Person = new PersonNameModel() { First = "Alan", Last = "Johnson" } }, "Alan Johnson");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("WithWithElse1", "{{#with person}}Person is present{{else}}Person is not present{{/with}}", _personModel)]
    public void WithWithElse()
    {
      ShouldRender("WithWithElse1", new PersonModel(), "Person is not present");
      ShouldRender("WithWithElse1", new PersonModel() { Person = new PersonNameModel() }, "Person is present");
    }



      /*
  describe('#with', function()
  {
    it('with', function() {
      var string = '{{#with person}}{{first}} {{last}}{{/with}}';
      shouldCompileTo(string, { person: { first: 'Alan', last: 'Johnson'} }, 'Alan Johnson');
    });
    it('with with function argument', function() {
      var string = '{{#with person}}{{first}} {{last}}{{/with}}';
      shouldCompileTo(string, { person: function() { return { first: 'Alan', last: 'Johnson'}; } }, 'Alan Johnson');
    });
    it('with with else', function() {
      var string = '{{#with person}}Person is present{{else}}Person is not present{{/with}}';
      shouldCompileTo(string, { }, 'Person is not present');
    });
    it('with provides block parameter', function() {
      var string = '{{#with person as |foo|}}{{foo.first}} {{last}}{{/with}}';
      shouldCompileTo(string, { person: { first: 'Alan', last: 'Johnson'} }, 'Alan Johnson');
    });
    it('works when data is disabled', function() {
      var template = CompilerContext.compile('{{#with person as |foo|}}{{foo.first}} {{last}}{{/with}}', { data: false});

      var result = template({ person: { first: 'Alan', last: 'Johnson'} });
      equals(result, 'Alan Johnson');
    });
  });*/

  }
}
