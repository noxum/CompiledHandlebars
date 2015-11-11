using CompiledHandlebars.CompilerTests.Helper;
using CompiledHandlebars.CompilerTests.TestViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests
{
  [TestClass]
  public class HelperTests : CompilerTestBase
  {
    private const string _marsModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.MarsModel}}";

    static HelperTests()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(HelperTests));
    }


    [CompiledHandlebarsHelperMethod]
    public static string ToUpper(string input)
    {
      return input.ToUpper();
    }

    [CompiledHandlebarsHelperMethod]
    public static string ToLower(string input)
    {
      return input.ToLower();
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("BasicHelperTest1", "{{ToUpper Name}}", _marsModel)]
    [RegisterHandlebarsTemplate("BasicHelperTest2", "{{ToUpper Name}}{{ToLower Name}}", _marsModel)]
    public void BasicHelperTest()
    {
      ShouldRender("BasicHelperTest1", MarsModelFactory.CreateFullMarsModel(), "MARS");
      ShouldRender("BasicHelperTest2", MarsModelFactory.CreateFullMarsModel(), "MARSmars");
    }

    [CompiledHandlebarsHelperMethod]
    public static string IsMoonOf(MarsModel mars, MoonModel moon) => $"{moon.Name} is a moon of {mars.Name}";

    [TestMethod]
    [RegisterHandlebarsTemplate("MultipleParametersHelperTest1", "{{IsMoonOf this Deimos}}", _marsModel)]
    public void MultipleParametersHelperTest()
    {
      ShouldRender("MultipleParametersHelperTest1", MarsModelFactory.CreateFullMarsModel(), "Deimos is a moon of Mars");
    }    

    [CompiledHandlebarsHelperMethod]
    public static string IndexPlusOne(int index) => (++index).ToString();    
    [CompiledHandlebarsHelperMethod]
    public static string BoolToYesNo(bool input) => input ? "yes" : "no";

    [TestMethod]
    [RegisterHandlebarsTemplate("SpecialParametersHelperTest1", "{{#each Plains}}{{IndexPlusOne @index}}:{{Name}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("SpecialParametersHelperTest2", "{{#each Plains}}first:{{BoolToYesNo @first}};last:{{BoolToYesNo @last}}{{/each}}", _marsModel)]
    public void SpecialParametersHelperTest()
    {
      ShouldRender("SpecialParametersHelperTest1", MarsModelFactory.CreateFullMarsModel(), "1:Acidalia Planitia2:Utopia Planitia");
      ShouldRender("SpecialParametersHelperTest2", MarsModelFactory.CreateFullMarsModel(), "first:yes;last:nofirst:no;last:yes");
    }

    [CompiledHandlebarsHelperMethod]
    public static string MarsHelper(MarsModel mars) => "this is mars";

    [TestMethod]
    [RegisterHandlebarsTemplate("ImplicitThisParameterTest1", "{{MarsHelper}}", _marsModel)]
    public void ImplicitThisParameterTest()
    {
      ShouldRender("ImplicitThisParameterTest1", MarsModelFactory.CreateFullMarsModel(), "this is mars");
    }

    private class CompiledHandlebarsHelperMethodAttribute : Attribute
    {
    }
  }
}
