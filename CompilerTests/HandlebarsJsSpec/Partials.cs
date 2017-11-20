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

	public class AnotherDudeModel
	{
		public string Name { get; set; }
		public string AnotherDude { get; set; }
	}

	[TestClass]
	public class Partials : CompilerTestBase
	{
		private const string _dudeListModel = "{{model System.Collections.Generic.IEnumerable<CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials.DudeModel>}}";
		private const string _dudeModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials.DudeModel}}";
		private const string _dudesModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials.DudesModel}}";
		private const string _anotherDudeModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials.AnotherDudeModel}}";


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

		[TestMethod]
		[RegisterHandlebarsTemplate("dude3", "{{model System.String}}{{.}}")]
		[RegisterHandlebarsTemplate("PartialWithStringContext1", "{{model System.String}}Dudes: {{>dude3 \"dudes\"}}")]
		public void PartialWithStringContext()
		{
			ShouldRender("PartialWithStringContext1", "", "Dudes: dudes");
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("url", "<a href=\"{{Url}}\">{{Url}}</a>", _dudeModel)]
		[RegisterHandlebarsTemplate("dude4", "{{Name}} {{> url}} ", _dudeModel)]
		[RegisterHandlebarsTemplate("PartialInAPartial1", "Dudes: {{#each Dudes}}{{>dude4}}{{/each}}", _dudesModel)]
		public void PartialInAPartial()
		{
			ShouldRender("PartialInAPartial1", new DudesModel()
			{
				Dudes = new List<DudeModel>()
		  {
			 new DudeModel() { Name = "Yehuda", Url = "http://yehuda" },
			 new DudeModel() {Name ="Alan", Url = "http://alan" }
		  }
			}, "Dudes: Yehuda <a href=\"http://yehuda\">http://yehuda</a> Alan <a href=\"http://alan\">http://alan</a> ");
		}


		[TestMethod]
		[RegisterHandlebarsTemplate("dude5", "{{Name}}", _anotherDudeModel)]
		[RegisterHandlebarsTemplate("PartialPrecedingASelector1", "Dudes: {{>dude5}} {{AnotherDude}}", _anotherDudeModel)]
		public void PartialPrecedingASelector()
		{
			ShouldRender("PartialPrecedingASelector1", new AnotherDudeModel() { Name = "Jeepers", AnotherDude = "Creepers" }, "Dudes: Jeepers Creepers");
		}


		[TestMethod]
		[RegisterHandlebarsTemplate("dude", "{{Name}}", _anotherDudeModel, "CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials.Shared")]
		[RegisterHandlebarsTemplate("PartialsWithSlashPaths1", "Dudes: {{> Shared/dude}}", _anotherDudeModel)]
		public void PartialsWithSlashPaths()
		{
			ShouldRender("PartialsWithSlashPaths1", new AnotherDudeModel() { Name = "Jeepers", AnotherDude = "Creepers" }, "Dudes: Jeepers");
		}


		[TestMethod]
		[RegisterHandlebarsTemplate("thing", "{{Name}}", _anotherDudeModel, "CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials.Shared.Dude")]
		[RegisterHandlebarsTemplate("PartialsWithSlashAndPointPaths1", "Dudes: {{> Shared/Dude.thing}}", _anotherDudeModel)]
		public void PartialsWithSlashAndPointPaths()
		{
			ShouldRender("PartialsWithSlashAndPointPaths1", new AnotherDudeModel() { Name = "Jeepers", AnotherDude = "Creepers" }, "Dudes: Jeepers");
		}
	}
}
