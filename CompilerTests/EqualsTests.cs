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
	public class EqualsTests : CompilerTestBase
	{
		private const string _marsModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.MarsModel}}";

		static EqualsTests()
		{
			assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(EqualsTests));
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("BasicEqualsTest1", "{{#equals Name Name}}EQUAL1{{else}}NOTEQUAL1{{/equals}}", _marsModel)]
		[RegisterHandlebarsTemplate("BasicEqualsTest2", "{{#equals Phobos Deimos}}EQUAL2{{else}}NOTEQUAL2{{/equals}}", _marsModel)]
		[RegisterHandlebarsTemplate("BasicEqualsTest3", "{{#equals Name \"Mars\"}}EQUAL3{{else}}NOTEQUAL3{{/equals}}", _marsModel)]
		public void BasicEqualsTest()
		{
			ShouldRender("BasicEqualsTest1", MarsModelFactory.CreateFullMarsModel(), "EQUAL1");
			ShouldRender("BasicEqualsTest1", new MarsModel(), "EQUAL1");
			ShouldRender("BasicEqualsTest2", MarsModelFactory.CreateFullMarsModel(), "NOTEQUAL2");
			ShouldRender("BasicEqualsTest2", new MarsModel(), "EQUAL2");
			ShouldRender("BasicEqualsTest3", MarsModelFactory.CreateFullMarsModel(), "EQUAL3");
			ShouldRender("BasicEqualsTest3", new MarsModel(), "NOTEQUAL3");
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("SpecialExpressionsEqualTest1", "{{#equals @root.Name Name}}EQUAL1{{/equals}}", _marsModel)]
		[RegisterHandlebarsTemplate("SpecialExpressionsEqualTest2", "{{#each Plains}}{{#equals @first @last}}impossible{{else}}ok{{/equals}}{{/each}}", _marsModel)]
		[RegisterHandlebarsTemplate("SpecialExpressionsEqualTest3", "{{#each Plains}}{{#equals @index ../MoonCount}}Second Plain is {{Name}}{{/equals}}{{/each}}", _marsModel)]
		public void SpecialExpressionsEqualTest()
		{
			ShouldRender("SpecialExpressionsEqualTest1", MarsModelFactory.CreateFullMarsModel(), "EQUAL1");
			ShouldRender("SpecialExpressionsEqualTest2", MarsModelFactory.CreateFullMarsModel(), "okok");
			var mm = MarsModelFactory.CreateFullMarsModel();
			mm.MoonCount = 1;
			ShouldRender("SpecialExpressionsEqualTest3", mm, "Second Plain is Utopia Planitia");
		}
	}
}
