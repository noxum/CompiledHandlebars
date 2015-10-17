using CompiledHandlebars.Compiler;
using CompiledHandlebars.CompilerTests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests
{
  [TestClass()]
  public class TruthynessTests : CompilerTestBase
  {
    private const string _marsModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.MarsModel}}";
    private const string _starModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.StarModel}}";
    static TruthynessTests()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(TruthynessTests));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("UnreachableCodeTest1", "{{#with Phobos}}{{#if this}}{{else}}{{/if}}{{/with}}", _marsModel, false)]
    [RegisterHandlebarsTemplate("UnreachableCodeTest2", "{{#with Phobos}}{{#unless this}}{{/unless}}{{/with}}", _marsModel, false)]
    public void UnreachableCodeTest()
    {
      ShouldRaiseError("UnreachableCodeTest1", HandlebarsTypeErrorKind.UnreachableCode);
      ShouldRaiseError("UnreachableCodeTest2", HandlebarsTypeErrorKind.UnreachableCode);

    }

  }
}
