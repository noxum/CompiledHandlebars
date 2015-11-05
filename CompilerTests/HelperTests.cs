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

    [TestMethod]
    [RegisterHandlebarsTemplate("BasicHelperTest1", "{{ToUpper Name}}", _marsModel)]
    public void BasicHelperTest()
    {
      ShouldRender("BasicHelperTest1", MarsModelFactory.CreateFullMarsModel(), "MARS");
    }

    private class CompiledHandlebarsHelperMethodAttribute : Attribute
    {
    }
  }
}
