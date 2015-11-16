using CompiledHandlebars.CompilerTests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials
{

  public class DudesModel
  {
    public List<DudeModel> Dudes { get; set; }
  }
  public class DudeModel
  {
    public string Name { get; set; }
    public string Url { get; set; }
  }

  [TestClass]
  public class Partials : CompilerTestBase
  {
    private const string _dudeListModel = "{{model System.Collections.Generic.IEnumerable<CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials.DudeModel>}}";
    private const string _dudeModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials.DudeModel}}";
    private const string _dudesModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials.DudesModel}}";


    static Partials()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(Partials));
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("dude1", "{{Name}} ({{Url}}) ", _dudeModel)]
    [RegisterHandlebarsTemplate("BasicPartials1", "Dudes: {{#each Dudes}}{{> dude1}}{{/each}}", _dudesModel)]
    public void BasicPartials()
    {
      ShouldRender("BasicPartials1", new DudesModel()
      {
        Dudes = new List<DudeModel>()
        {
          new DudeModel() { Name = "Yehuda", Url = "http://yehuda" },
          new DudeModel() {Name ="Alan", Url = "http://alan" }
        }
      }, "Dudes: Yehuda (http://yehuda) Alan (http://alan) ");
    }


    [TestMethod]
    [RegisterHandlebarsTemplate("dude2", "{{#each this}}{{Name}} ({{Url}}) {{/each}}", _dudeListModel)]
    [RegisterHandlebarsTemplate("PartialsWithContext1", "Dudes: {{>dude2 Dudes}}", _dudesModel)]
    public void PartialsWithContext()
    {
      ShouldRender("PartialsWithContext1", new DudesModel()
      {
        Dudes = new List<DudeModel>()
        {
          new DudeModel() { Name = "Yehuda", Url = "http://yehuda" },
          new DudeModel() {Name ="Alan", Url = "http://alan" }
        }
      }, "Dudes: Yehuda (http://yehuda) Alan (http://alan) ");
    }
  }
}
