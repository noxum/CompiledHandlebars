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
  public class LayoutTests : CompilerTestBase
  {
    private const string _iPageModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.IPageModel}}";
    private const string _pageModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.PageModel}}";

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

    [TestMethod]
    [RegisterHandlebarsTemplate("LayoutsWithInterfaceModelTest1", "<head><title>{{Title}}</title></head><body>{{body}}</body>", _iPageModel)]
    [RegisterHandlebarsTemplate("LayoutsWithInterfaceModelTest2", "{{layout LayoutsWithInterfaceModelTest1}}<h1>{{Headline}}</h1>", _pageModel)]
    public void LayoutsWithInterfaceModelTest()
    {
      ShouldRender("LayoutsWithInterfaceModelTest2", new PageModel() { Headline = "Planet Number Four: Mars", Title = "Mars" },
        "<head><title>Mars</title></head><body><h1>Planet Number Four: Mars</h1></body>");
    }

  }
}
