using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompiledHandlebars.Compiler;
using CompiledHandlebars.CompilerTests.Helper;

namespace CompiledHandlebars.CompilerTests
{
  [TestClass]
  public class ParserTests : CompilerTestBase
  {
    //Acts as Dummy inside this class
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
    [RegisterHandlebarsTemplate("MalformedModelTest2", "{{model Venus", false)]
    [RegisterHandlebarsTemplate("MalformedModelTest3", "{{Mars}}", false)]
    [RegisterHandlebarsTemplate("MalformedModelTest4", "<solarSystem></solarSystem>", false)]
    [RegisterHandlebarsTemplate("MalformedModelTest5", "{{model }}", false)]
    [RegisterHandlebarsTemplate("MalformedModelTest6", "{{model Mars..Utopia}}", false)]
    public void MalformedModelTest()

    {
      ShouldRaiseError("MalformedModelTest1", HandlebarsSyntaxErrorKind.MalformedModelToken);
      ShouldRaiseError("MalformedModelTest2", HandlebarsSyntaxErrorKind.MissingModelToken);
      ShouldRaiseError("MalformedModelTest3", HandlebarsSyntaxErrorKind.MissingModelToken);
      ShouldRaiseError("MalformedModelTest4", HandlebarsSyntaxErrorKind.MissingModelToken);
      ShouldRaiseError("MalformedModelTest5", HandlebarsSyntaxErrorKind.MalformedModelToken);
      ShouldRaiseError("MalformedModelTest6", HandlebarsSyntaxErrorKind.MalformedMemberExpression);
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
      ShouldRaiseError("UnexpectedCharacterTest1", HandlebarsSyntaxErrorKind.UnexpectedCharacter);
    }


  }

}

