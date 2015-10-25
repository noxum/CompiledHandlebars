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
  public class SpecialKeywordsTest : CompilerTestBase
  {
    private const string _marsModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.MarsModel}}";    
    static SpecialKeywordsTest()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(SpecialKeywordsTest));
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("TestRoot1", "{{#each Rovers}}{{@root.Name}}{{/each}}", _marsModel)]
    public void TestRoot()
    {
      ShouldRender("TestRoot1", MarsModelFactory.CreateFullMarsModel(), "MarsMars");
    }

  }
}
