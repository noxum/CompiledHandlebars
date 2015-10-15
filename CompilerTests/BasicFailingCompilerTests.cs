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
    [RegisterHandlebarsTemplate("ContextErrorTest1", "{{../Name}}", _marsModel, false)]
    [RegisterHandlebarsTemplate("ContextErrorTest2", "{{Phobos/../../Name}}", _marsModel, false)]
    public void ContextErrorTest()
    {
      ShouldRaiseError("ContextErrorTest1", Compiler.HandlebarsTypeErrorKind.EmptyContextStack);
      ShouldRaiseError("ContextErrorTest2", Compiler.HandlebarsTypeErrorKind.EmptyContextStack);
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("UnknownMemberTest1", "{{Europa}}", _marsModel, false)]
    [RegisterHandlebarsTemplate("UnknownMemberTest2", "{{#if Titan}}{{/if}}", _marsModel, false)]
    public void UnknownMemberTest()
    {
      ShouldRaiseError("UnknownMemberTest1", Compiler.HandlebarsTypeErrorKind.UnknownMember);
      ShouldRaiseError("UnknownMemberTest2", Compiler.HandlebarsTypeErrorKind.UnknownMember);
    }


  }
}
