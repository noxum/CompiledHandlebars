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
    [RegisterHandlebarsTemplate("RootTest1", "{{#each Rovers}}{{@root.Name}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("RootTest2", "{{#each Rovers}}{{> RootTest3 @root}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("RootTest3", "{{Name}}", _marsModel)]
    public void RootTest()
    {
      ShouldRender("RootTest1", MarsModelFactory.CreateFullMarsModel(), "MarsMars");
      ShouldRender("RootTest2", MarsModelFactory.CreateFullMarsModel(), "MarsMars");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("FirstTest1", "{{#each Plains}}{{#if @first}}{{Name}}{{/if}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("FirstTest2", "{{#each Plains}}{{#if @first}}{{else}}{{Name}}{{/if}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("FirstTest3", "{{#each Plains}}{{#unless @first}}{{Name}}{{/unless}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("FirstTest4", "{{#each Plains}}{{#unless @first}}{{else}}{{Name}}{{/unless}}{{/each}}", _marsModel)]
    public void FirstTest()
    {
      ShouldRender("FirstTest1", MarsModelFactory.CreateFullMarsModel(), "Acidalia Planitia");
      ShouldRender("FirstTest2", MarsModelFactory.CreateFullMarsModel(), "Utopia Planitia");
      ShouldRender("FirstTest3", MarsModelFactory.CreateFullMarsModel(), "Utopia Planitia");
      ShouldRender("FirstTest4", MarsModelFactory.CreateFullMarsModel(), "Acidalia Planitia");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("LastTest1", "{{#each Plains}}{{#if @last}}{{Name}}{{/if}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("LastTest2", "{{#each Plains}}{{#if @last}}{{else}}{{Name}}{{/if}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("LastTest3", "{{#each Plains}}{{#unless @last}}{{Name}}{{/unless}}{{/each}}", _marsModel)]
    [RegisterHandlebarsTemplate("LastTest4", "{{#each Plains}}{{#unless @last}}{{else}}{{Name}}{{/unless}}{{/each}}", _marsModel)]
    public void LastTest()
    {
      ShouldRender("LastTest1", MarsModelFactory.CreateFullMarsModel(), "Utopia Planitia");
      ShouldRender("LastTest2", MarsModelFactory.CreateFullMarsModel(), "Acidalia Planitia");
      ShouldRender("LastTest3", MarsModelFactory.CreateFullMarsModel(), "Acidalia Planitia");
      ShouldRender("LastTest4", MarsModelFactory.CreateFullMarsModel(), "Utopia Planitia");
    }

  }
}
