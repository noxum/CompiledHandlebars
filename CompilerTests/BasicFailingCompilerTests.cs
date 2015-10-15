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
  public class BasicFailingCompilerTests : CompilerTestBase
  {
    private const string _marsModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.MarsModel}}";

    static BasicFailingCompilerTests()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(BasicFailingCompilerTests));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("UnknownViewModelTest", "{{model Mars}}", false)]
    public void UnknownViewModelTest()
    {
      ShouldRaiseError("UnknownViewModelTest", Compiler.HandlebarsTypeErrorKind.UnknownViewModel);
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("ContextErrorTests1", "{{../Name}}", _marsModel, false)]
    [RegisterHandlebarsTemplate("ContextErrorTests2", "{{Phobos/../../Name}}", _marsModel, false)]
    public void ContextErrorTests()
    {
      ShouldRaiseError("ContextErrorTests1", Compiler.HandlebarsTypeErrorKind.EmptyContextStack);
      ShouldRaiseError("ContextErrorTests2", Compiler.HandlebarsTypeErrorKind.EmptyContextStack);
    }


  }
}
