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

  [TestClass()]
  public class InterfacedModelsTests : CompilerTestBase
  {

    static InterfacedModelsTests()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(InterfacedModelsTests));  
    }


    [TestMethod]
    [RegisterHandlebarsTemplate("IBaseIntegerTest", "{{model CompiledHandlebars.CompilerTests.TestViewModels.IBase}}{{AnotherInt}}")]
    public void IBaseIntegerTest()
    {
      ShouldRender("IBaseIntegerTest", BaseModelFactory.CreateFullBaseModel(),"20");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("IDerivedIntegerTest", "{{model CompiledHandlebars.CompilerTests.TestViewModels.IDerived}}{{AnotherInt}}")]
    public void IDerivedIntegerTest()
    {
      ShouldRender("IDerivedIntegerTest", DerivedModelFactory.CreateFullDerivedModel(), "10");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("IBaseStringTest", "{{model CompiledHandlebars.CompilerTests.TestViewModels.IBase}}{{AnotherString}}")]
    public void IBaseStringTest()
    {
      ShouldRender("IBaseStringTest", BaseModelFactory.CreateFullBaseModel(), "everything");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("IDerivedStringTest", "{{model CompiledHandlebars.CompilerTests.TestViewModels.IDerived}}{{AnotherString}}")]
    public void IDerivedStringTest()
    {
      ShouldRender("IDerivedStringTest", DerivedModelFactory.CreateFullDerivedModel(), "nothing");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("IBaseListTest", "{{model CompiledHandlebars.CompilerTests.TestViewModels.IBase}}{{#each AnotherList}}{{this}}{{/each}}")]
    public void IBaseListTest()
    {
      ShouldRender("IBaseListTest", BaseModelFactory.CreateFullBaseModel(), "thisStringthatString");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("IDerivedListTest", "{{model CompiledHandlebars.CompilerTests.TestViewModels.IDerived}}{{#each AnotherList}}{{this}}{{/each}}")]
    public void IDerivedListTest()
    {
      ShouldRender("IDerivedListTest", DerivedModelFactory.CreateFullDerivedModel(), "firstStringsecondString");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("IBaseClassTest", "{{model CompiledHandlebars.CompilerTests.TestViewModels.IBase}}{{AnotherClass.DummyString}}")]
    public void IBaseClassTest()
    {
      ShouldRender("IBaseClassTest", BaseModelFactory.CreateFullBaseModel(), "justAnotherString");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("IDerivedClassTest", "{{model CompiledHandlebars.CompilerTests.TestViewModels.IDerived}}{{AnotherClass.DummyString}}")]
    public void IDerivedClassTest()
    {
      ShouldRender("IDerivedClassTest", DerivedModelFactory.CreateFullDerivedModel(), "justAnotherString");
    }
  }
}
