using CompiledHandlebars.Compiler;
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
  public class TruthynessTests : CompilerTestBase
  {
    private const string _marsModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.MarsModel}}";
    private const string _starModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.StarModel}}";
    private const string _namingConfModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.NamingConflictsModel}}";
    static TruthynessTests()
    {
      assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(TruthynessTests));
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("UnreachableCodeTest1", "{{#with Phobos}}{{#if this}}{{else}}{{/if}}{{/with}}", _marsModel, false)]
    [RegisterHandlebarsTemplate("UnreachableCodeTest2", "{{#with Phobos}}{{#unless this}}{{else}}{{/unless}}{{/with}}", _marsModel, false)]
    public void UnreachableCodeTest()
    {
      ShouldRaiseError("UnreachableCodeTest1", HandlebarsTypeErrorKind.UnreachableCode);
      ShouldRaiseError("UnreachableCodeTest2", HandlebarsTypeErrorKind.UnreachableCode);
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("IterateOverNullTest1", "{{#each Planets}}{{Name}};{{/each}}", _starModel)]
    [RegisterHandlebarsTemplate("IterateOverNullTest2", "{{#each Planets}}{{#each Moons}}{{Name}}{{/each}}{{/each}}", _starModel)]
    public void IterateOverNullTest()
    {
      ShouldRender("IterateOverNullTest1", default(StarModel), "");
      ShouldRender("IterateOverNullTest2", CelestialBodyFactory.CreateSolarSystem(), "MoonDeimosPhobos");
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("RedundantTruthynessCheckTest1", "{{#with Phobos}}{{#if Name}}{{Name}}{{/if}}{{/with}}", _marsModel)]
    [RegisterHandlebarsTemplate("RedundantTruthynessCheckTest2", "{{#with Rovers}}{{#each this}}{{Value.Name}}{{/each}}{{/with}}", _marsModel)]
    [RegisterHandlebarsTemplate("RedundantTruthynessCheckTest3", "{{#if Rovers}}{{#each Rovers}}{{Value.Name}}{{/each}}{{/if}}", _marsModel)]
    public void RedundantTruthynessCheckTest()
    {
      ShouldRender("RedundantTruthynessCheckTest1", MarsModelFactory.CreateFullMarsModel(), "Phobos");
      ShouldContainCode("RedundantTruthynessCheckTest1", @"IsTruthy\(viewModel\)", 1);
      ShouldContainCode("RedundantTruthynessCheckTest1", @"IsTruthy\(viewModel.Phobos\)", 1);
      ShouldContainCode("RedundantTruthynessCheckTest1", @"IsTruthy\(viewModel.Phobos.Name\)", 1);
      ShouldRender("RedundantTruthynessCheckTest2", MarsModelFactory.CreateFullMarsModel(), "OpportunityCuriosity");
      ShouldContainCode("RedundantTruthynessCheckTest2", @"IsTruthy\(viewModel\)", 1);
      ShouldContainCode("RedundantTruthynessCheckTest2", @"IsTruthy\(viewModel.Rovers\)", 1);
      ShouldRender("RedundantTruthynessCheckTest3", MarsModelFactory.CreateFullMarsModel(), "OpportunityCuriosity");
      ShouldContainCode("RedundantTruthynessCheckTest3", @"IsTruthy\(viewModel\)", 1);
      ShouldContainCode("RedundantTruthynessCheckTest3", @"IsTruthy\(viewModel.Rovers\)", 1);
    }

    [TestMethod()]
    [RegisterHandlebarsTemplate("NamingConflictsInTruthynessQueriesTest1", "{{#if Items}}{{#if ItemsTitle}}<h1>{{ItemsTitle}}</h1>{{/if}}{{/if}}", _namingConfModel)]
    public void NamingConflictsInTruthynessQueriesTest()
    {
      ShouldRender("NamingConflictsInTruthynessQueriesTest1", new NamingConflictsModel() { Items = new List<string>() { "Item1" }, ItemsTitle = "Title" }, "<h1>Title</h1>");
    }

    [TestMethod]
    [RegisterHandlebarsTemplate("CaretIsElseTest1", "{{#if Name}}Name{{^}}NoName{{/if}}", _marsModel)]
    public void CaretIsElseTest()
    {
      ShouldRender("CaretIsElseTest1", default(MarsModel), "NoName");
      ShouldRender("CaretIsElseTest1", MarsModelFactory.CreateFullMarsModel(), "Name");
    }


    [TestMethod]
    [RegisterHandlebarsTemplate("UnnecessaryIfTest1", "{{#if Phobos}}{{#if this}}{{Phobos.Name}}{{/if}}{{/if}}",_marsModel)]
    [RegisterHandlebarsTemplate("UnnecessaryIfTest2", "{{#if Phobos}}{{#if this}}{{Phobos.Name}}{{/if}}{{else}}{{#if this}}No Phobos{{/if}}{{/if}}", _marsModel)]
    public void UnnecessaryIfTest()
    {
      ShouldRender("UnnecessaryIfTest1", MarsModelFactory.CreateFullMarsModel(), "Phobos");
      ShouldRender("UnnecessaryIfTest2", MarsModelFactory.CreateFullMarsModel(), "Phobos");
      ShouldRender("UnnecessaryIfTest2", new MarsModel(), "No Phobos");
    }
  }
}
