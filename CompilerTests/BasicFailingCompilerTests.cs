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
  class BasicFailingCompilerTests : CompilerTestBase
  {
    private const string _marsModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.MarsModel}}";

    static BasicFailingCompilerTests()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(BasicFailingCompilerTests));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("UnknownViewModel", "{{model Mars}}", false)]
    public void BasicTest()
    {
      ShouldRaiseError("UnknownViewModel", Compiler.HandlebarsTypeErrorKind.UnknownType);
    }


  }
}
