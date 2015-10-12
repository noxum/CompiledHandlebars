using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CompiledHandlebars.Compiler;

namespace CompiledHandlebars.CompilerTests
{
  [TestClass]
  public class SyntacticAnalysisTests
  {
    private readonly HbsParser _parser = new HbsParser();
    [TestMethod]
    public void BasicSuccess()
    {
      try
      {
        Assert.IsNotNull(_parser.Parse("{{model SolarSystem.Mars}}"), "Empty Template");
      } catch (Exception e)
      {
        Assert.Fail(e.Message);
      }
    }

    [TestMethod]
    public void BasicFail()
    {

      ShouldThrowException("{{model}}");
      ShouldThrowException("{{model Venus");
      ShouldThrowException("{{Mars}}");
      ShouldThrowException("<solarSystem></solarSystem>");
       
    }

    [TestMethod]
    public void MemberExpressionFail()
    {
      ShouldThrowException("{{model Mars}}{{Plantatia.Utopia.}}");
      ShouldThrowException("{{model Mars}}{{Plantatia..Utopia}}");
    }

    [TestMethod]
    public void WithBlockFail()
    {
      ShouldThrowException("{{model Mars}}{{#with}}");
      ShouldThrowException("{{model Mars}}{{#with Sun}}");
      ShouldThrowException("{{model Mars}}{{#with Sun}}{{#with}}");
      ShouldThrowException("{{model Mars}}{{#with Sun}}{{/witz}}");
    }

    private void ShouldThrowException(string template)
    {
      try
      {
        _parser.Parse(template);        
        Assert.Fail();
      }catch(HandlebarsSyntaxError)
      {

      }catch(Exception)
      {
        Assert.Fail();
      }
    }
  }
}
