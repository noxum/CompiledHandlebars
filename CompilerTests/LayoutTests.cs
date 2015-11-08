using CompiledHandlebars.CompilerTests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests
{
  [TestClass]
  public class LayoutTests : CompilerTestBase
  {
    static LayoutTests()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(LayoutTests));
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("BasicLayoutTest1", "{{model System.String}}<h1>{{body}}</h1>")]
    [RegisterHandlebarsTemplate("BasicLayoutTest2", "{{model System.String}}{{layout BasicLayoutTest1}}{{.}}")]
    public void BasicLayoutTest()
    {
      ShouldRender("BasicLayoutTest2", "Hans", "<h1>Hans</h1>");
    }

  }
}
