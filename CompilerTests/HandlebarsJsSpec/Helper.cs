using CompiledHandlebars.CompilerTests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Helper
{
  [TestClass]
  public class Helper : CompilerTestBase
  {
    static Helper()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(Helper));
    }

    public static string Link()

    [TestMethod]
    public void HelperWithComplexLookup()
    {

    }

    private class CompiledHandlebarsHelperMethodAttribute : Attribute { }
  }
}
