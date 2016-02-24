using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompiledHandlebars.Compiler;
using CompiledHandlebars.CompilerTests.Helper;
using CompiledHandlebars.CompilerTests.TestViewModels;

namespace CompiledHandlebars.CompilerTests
{
  [TestClass]
  public class ParserTests : CompilerTestBase
  {
    private const string _marsModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.MarsModel}}";

    static ParserTests()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(ParserTests));
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("emptyTemplate", _marsModel, false)]
    public void EmptyTemplateTest()
    {
      ShouldCompileWithoutError("emptyTemplate");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("MalformedModelTest1", "{{model}}", false)]
    [RegisterHandlebarsTemplate("MalformedModelTest3", "{{Mars}}", false)]
    [RegisterHandlebarsTemplate("MalformedModelTest5", "{{model }}", false)]
    public void MalformedModelTest()
    {
      ShouldRaiseError("MalformedModelTest1", HandlebarsSyntaxErrorKind.MalformedModelToken);
      ShouldRaiseError("MalformedModelTest3", HandlebarsSyntaxErrorKind.MissingModelToken);
      ShouldRaiseError("MalformedModelTest5", HandlebarsSyntaxErrorKind.MalformedModelToken);
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("MalformedMemberExpressionTest1", "{{Mars.UtopiaPlanitia.}}", _marsModel, false)]
    [RegisterHandlebarsTemplate("MalformedMemberExpressionTest2", "{{Mars..UtopiaPlanitia}}", _marsModel, false)]
    public void MalformedMemberExpressionTest()
    {
      ShouldRaiseError("MalformedMemberExpressionTest1", HandlebarsSyntaxErrorKind.MalformedMemberExpression);
      ShouldRaiseError("MalformedMemberExpressionTest2", HandlebarsSyntaxErrorKind.MalformedMemberExpression);
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("MalformedWithBlockTest1", "{{#with}}", _marsModel, false)]
    [RegisterHandlebarsTemplate("MalformedWithBlockTest2", "{{#with Sun}}", _marsModel, false)]
    [RegisterHandlebarsTemplate("MalformedWithBlockTest3", "{{#with Sun}}{{#with}}", _marsModel, false)]
    [RegisterHandlebarsTemplate("MalformedWithBlockTest4", "{{#with Sun}}{{/witz}}", _marsModel, false)]
    [RegisterHandlebarsTemplate("MalformedWithBlockTest5", "{{#with Sun}}{{#unless this}}{{/if}}{{/with}}", _marsModel, false)]
    public void MalformedWithBlockTest()
    {
      ShouldRaiseError("MalformedWithBlockTest1", HandlebarsSyntaxErrorKind.MissingMemberExpression);
      ShouldRaiseError("MalformedWithBlockTest2", HandlebarsSyntaxErrorKind.MalformedBlock);
      ShouldRaiseError("MalformedWithBlockTest3", HandlebarsSyntaxErrorKind.MissingMemberExpression);
      ShouldRaiseError("MalformedWithBlockTest4", HandlebarsSyntaxErrorKind.MalformedBlock);
      ShouldRaiseError("MalformedWithBlockTest5", HandlebarsSyntaxErrorKind.MalformedBlock);
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("MalformedPartialCallTest1", "{{> !#Partial}}", _marsModel, false)]
    [RegisterHandlebarsTemplate("MalformedPartialCallTest2", "{{>}}", _marsModel, false)]
    public void MalformedPartialCallTest()
    {
      ShouldRaiseError("MalformedPartialCallTest1", HandlebarsSyntaxErrorKind.MalformedPartialCallToken);
      ShouldRaiseError("MalformedPartialCallTest2", HandlebarsSyntaxErrorKind.MalformedPartialCallToken);
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("UnknownSpecialExpressionTest1", "{{@mars}}", _marsModel, false)]
    public void UnkownSpecialExpressionTest()
    {
      ShouldRaiseError("UnknownSpecialExpressionTest1", HandlebarsSyntaxErrorKind.UnknownSpecialExpression);
    }    

    [TestMethod]
    [RegisterHandlebarsTemplate("UnexpectedCharacterTest1", "{{$mars}}", _marsModel, false)]    
    public void UnexpectedCharacterTest()
    {
      ShouldRaiseError("UnexpectedCharacterTest1", HandlebarsSyntaxErrorKind.MalformedHandlebarsToken);
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("NestedCommentsTest1", "{{#if Phobos}}{{!-- NestedComment--}}{{/if}}", _marsModel)]
    [RegisterHandlebarsTemplate("NestedCommentsTest2", "{{#if Phobos}}{{!-- {{#if Token~}}--}}{{/if}}", _marsModel)]
    [RegisterHandlebarsTemplate("NestedCommentsTest3", "{{#if Phobos}}{{!-- {{#if Token~}}--}}{{#if Deimos}}Deimos{{/if}}{{/if}}", _marsModel)]
    [RegisterHandlebarsTemplate("NestedCommentsTest4", "{{#with this}}{{#if Phobos}}{{!-- {{#if Token~}}--}}{{#if Deimos}}Deimos{{/if}}{{/if}}{{/with}}", _marsModel)]
    [RegisterHandlebarsTemplate("NestedCommentsTest5", "{{#if Phobos}}\n{{!-- {{#if Token}}--}}{{/if}}", _marsModel)]
    [RegisterHandlebarsTemplate("NestedCommentsTest6", "{{!-- {{#if ../../ContentItems}} --}}", _marsModel)]
    public void NestedCommentsTest()
    {
      ShouldRender("NestedCommentsTest1", MarsModelFactory.CreateFullMarsModel(), "");
      ShouldRender("NestedCommentsTest2", MarsModelFactory.CreateFullMarsModel(), "");
      ShouldRender("NestedCommentsTest3", MarsModelFactory.CreateFullMarsModel(), "Deimos");
      ShouldRender("NestedCommentsTest4", MarsModelFactory.CreateFullMarsModel(), "Deimos");
      ShouldRender("NestedCommentsTest5", MarsModelFactory.CreateFullMarsModel(), "\n");
      ShouldRender("NestedCommentsTest6", MarsModelFactory.CreateFullMarsModel(), "");
    }

  }

}

