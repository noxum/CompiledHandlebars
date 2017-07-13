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
	public class EachTests : CompilerTestBase
	{
		private const string _pageModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.PageModel}}";
		private const string _pageListModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.PageListModel}}";
		private const string _inheritedListModel = "{{model CompiledHandlebars.CompilerTests.TestViewModels.InheritedListModel}}";


		static EachTests()
		{
			assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(EachTests));
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("EachOverObjectTest1", "{{#each this}}{{this}}{{/each}}", _pageModel)]
		[RegisterHandlebarsTemplate("EachOverObjectTest2", "{{#each this}}{{@index}}{{this}}{{/each}}", _pageModel)]
		[RegisterHandlebarsTemplate("EachOverObjectTest3", "{{#each this}}{{#if @first}}{{this}}{{/if}}{{/each}}", _pageModel)]
		[RegisterHandlebarsTemplate("EachOverObjectTest4", "{{#each this}}{{#if @last}}{{this}}{{/if}}{{/each}}", _pageModel)]
		[RegisterHandlebarsTemplate("EachOverObjectTest5", "{{#each this}}{{@key}}:{{this}}{{/each}}", _pageModel)]

		public void EachOverObjectTest()
		{
			ShouldRender("EachOverObjectTest1", new PageModel() { Title = "Title", Headline = "Headline" }, "TitleHeadline");
			ShouldRender("EachOverObjectTest2", new PageModel() { Title = "Title", Headline = "Headline" }, "0Title1Headline");
			ShouldRender("EachOverObjectTest3", new PageModel() { Title = "Title", Headline = "Headline" }, "Title");
			ShouldRender("EachOverObjectTest4", new PageModel() { Title = "Title", Headline = "Headline" }, "Headline");
			ShouldRender("EachOverObjectTest5", new PageModel() { Title = "Title", Headline = "Headline" }, "Title:TitleHeadline:Headline");
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("NestedEachLoopsTest1", "{{#each Items}}{{#each this}}{{this}}{{/each}}{{/each}}", _pageListModel)]
		[RegisterHandlebarsTemplate("NestedEachLoopsTest2", "{{#each Items}}{{#each this}}{{#if @first}}{{this}}{{/if}}{{/each}}{{/each}}", _pageListModel)]
		[RegisterHandlebarsTemplate("NestedEachLoopsTest3", "{{#each Items}}{{#each this}}{{#if @last}}{{this}}{{/if}}{{/each}}{{/each}}", _pageListModel)]
		[RegisterHandlebarsTemplate("NestedEachLoopsTest4", "{{#each Items}}{{#if @first}}{{#each this}}{{this}}{{/each}}{{/if}}{{/each}}", _pageListModel)]
		[RegisterHandlebarsTemplate("NestedEachLoopsTest5", "{{#each Items}}{{#if @first}}{{#each this}}{{#if @first}}{{this}}{{/if}}{{/each}}{{/if}}{{/each}}", _pageListModel)]
		[RegisterHandlebarsTemplate("NestedEachLoopsTest6", "{{#each Items}}{{@index}}{{#each this}}{{@index}}{{this}}{{/each}}{{/each}}", _pageListModel)]

		public void NestedEachLoopsTest()
		{
			var pageList = new PageListModel()
			{
				Items = new List<PageModel>()
		  {
			 new PageModel() {Title = "A", Headline="B" },
			 new PageModel() {Title = "C", Headline="D" },
			 new PageModel() {Title = "E", Headline="F" }
		  }
			};
			ShouldRender("NestedEachLoopsTest1", pageList, "ABCDEF");
			ShouldRender("NestedEachLoopsTest2", pageList, "ACE");
			ShouldRender("NestedEachLoopsTest3", pageList, "BDF");
			ShouldRender("NestedEachLoopsTest4", pageList, "AB");
			ShouldRender("NestedEachLoopsTest5", pageList, "A");
			ShouldRender("NestedEachLoopsTest6", pageList, "00A1B10C1D20E1F");

		}

		[TestMethod]
		[RegisterHandlebarsTemplate("InheritedListTest1", "{{#each this}}{{Title}}-{{Headline}}{{/each}}", _inheritedListModel)]
		public void InheritedListTest()
		{
			var inheritedList = new InheritedListModel();
			inheritedList.Add(new PageModel() { Title = "A", Headline = "B" });
			inheritedList.Add(new PageModel() { Title = "C", Headline = "D" });
			inheritedList.Add(new PageModel() { Title = "E", Headline = "F" });
			ShouldRender("InheritedListTest1", inheritedList, "A-BC-DE-F");
		}

	}
}
