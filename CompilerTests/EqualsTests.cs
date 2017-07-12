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
		[RegisterHandlebarsTemplate("BasicEqualsTest1", "{{#equals Name Name}}EQUAL{{else}}NOTEQUAL{{/equal}}", _marsModel)]
		[RegisterHandlebarsTemplate("BasicEqualsTest2", "{{#equals Phobos Deimos}}EQUAL{{else}}NOTEQUAL{{/equal}}", _marsModel)]
		[RegisterHandlebarsTemplate("BasicEqualsTest3", "{{#equals Name \"Mars\"}}EQUAL{{else}}NOTEQUAL{{/equal}}", _marsModel)]
		public void BasicEqualsTest()
		{
			ShouldRender("BasicEqualsTest1", MarsModelFactory.CreateFullMarsModel(), "EQUAL");
			ShouldRender("BasicEqualsTest1", (MarsModel)null, "EQUAL");
			ShouldRender("BasicEqualsTest2", MarsModelFactory.CreateFullMarsModel(), "NOTEQUAL");
			ShouldRender("BasicEqualsTest2", (MarsModel)null, "EQUAL");
			ShouldRender("BasicEqualsTest3", MarsModelFactory.CreateFullMarsModel(), "EQUAL");
			ShouldRender("BasicEqualsTest3", (MarsModel)null, "NOTEQUAL");
		}
	}
}
