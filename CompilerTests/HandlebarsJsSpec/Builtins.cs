using CompiledHandlebars.CompilerTests.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins
{
	public class GoodbyeModel
	{
		public GoodbyeModel Foo { get; set; }
		public string StrGoodbye { get; set; }
		public bool BoolGoodbye { get; set; }
		public int IntGoodbye { get; set; }
		public List<string> ListGoodbye { get; set; }
		public string World { get; set; }
	}

	public class TextListModel
	{
		public List<TextModel> Goodbyes { get; set; }
		public string World { get; set; }
	}
	public class TextModel
	{
		public string Text { get; set; }
	}
	public class ABCModel
	{
		public TextModel A { get; set; }
		public TextModel B { get; set; }
		public TextModel C { get; set; }
	}

	public class FooBarModel
	{
		public TextModel Foo { get; set; }
		public TextModel Bar { get; set; }
	}

	public class MultiTextModel1
	{
		public ABCModel Goodbyes { get; set; }
		public string World { get; set; }
	}

	public class MultiTextModel2
	{
		public FooBarModel Goodbyes { get; set; }
		public string World { get; set; }
	}

	public class PersonModel
	{
		public PersonNameModel Person { get; set; }
	}

	public class PersonNameModel
	{
		public string First { get; set; }
		public string Last { get; set; }
	}

	public class LetterListModel
	{
		public List<string> Letters { get; set; }
	}


	/// <summary>
	/// https://github.com/wycats/handlebars.js/blob/master/spec/builtins.js
	/// </summary>
	[TestClass]
	public class BuiltinsTest : CompilerTestBase
	{
		private const string _personModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.PersonModel}}";
		private const string _goodbyeModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.GoodbyeModel}}";
		private const string _textListModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.TextListModel}}";
		private const string _letterListModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.LetterListModel}}";
		private const string _multiTextModel = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.MultiTextModel1}}";
		private const string _multiTextModel2 = "{{model CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.MultiTextModel2}}";


		static BuiltinsTest()
		{
			assemblyWithCompiledTemplates = CompileTemplatesToAssembly(typeof(BuiltinsTest));
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("If1", "{{#if StrGoodbye}}GOODBYE {{/if}}cruel {{World}}!", _goodbyeModel)]
		[RegisterHandlebarsTemplate("If2", "{{#if IntGoodbye}}GOODBYE {{/if}}cruel {{World}}!", _goodbyeModel)]
		[RegisterHandlebarsTemplate("If3", "{{#if BoolGoodbye}}GOODBYE {{/if}}cruel {{World}}!", _goodbyeModel)]
		[RegisterHandlebarsTemplate("If4", "{{#if ListGoodbye}}GOODBYE {{/if}}cruel {{World}}!", _goodbyeModel)]
		[RegisterHandlebarsTemplate("If5", "{{#with Foo}}{{#if BoolGoodbye}}GOODBYE cruel {{../World}}!{{/if}}{{/with}}", _goodbyeModel)]
		public void If()
		{
			ShouldRender("If3", new GoodbyeModel() { BoolGoodbye = true, World = "world" }, "GOODBYE cruel world!");
			ShouldRender("If1", new GoodbyeModel() { StrGoodbye = "dummy", World = "world" }, "GOODBYE cruel world!");
			ShouldRender("If3", new GoodbyeModel() { BoolGoodbye = false, World = "world" }, "cruel world!");
			ShouldRender("If3", new GoodbyeModel() { World = "world" }, "cruel world!");
			ShouldRender("If4", new GoodbyeModel() { ListGoodbye = new List<string>() { "foo" }, World = "world" }, "GOODBYE cruel world!");
			ShouldRender("If4", new GoodbyeModel() { ListGoodbye = new List<string>() { }, World = "world" }, "cruel world!");
			ShouldRender("If2", new GoodbyeModel() { IntGoodbye = 0, World = "world" }, "cruel world!");
			ShouldRender("If5", new GoodbyeModel() { World = "world", Foo = new GoodbyeModel() { BoolGoodbye = true } }, "GOODBYE cruel world!");
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("With1", "{{#with Person}}{{First}} {{Last}}{{/with}}", _personModel)]
		public void With()
		{
			ShouldRender("With1", new PersonModel() { Person = new PersonNameModel() { First = "Alan", Last = "Johnson" } }, "Alan Johnson");
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("WithWithElse1", "{{#with Person}}Person is present{{else}}Person is not present{{/with}}", _personModel)]
		public void WithWithElse()
		{
			ShouldRender("WithWithElse1", new PersonModel(), "Person is not present");
			ShouldRender("WithWithElse1", new PersonModel() { Person = new PersonNameModel() }, "Person is present");
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("Each1", "{{#each Goodbyes}}{{Text}}! {{/each}}cruel {{World}}!", _textListModel)]
		public void Each()
		{
			ShouldRender("Each1", new TextListModel()
			{
				Goodbyes = new List<TextModel>()
		  {
			 new TextModel() { Text = "goodbye"},
			 new TextModel() { Text = "Goodbye"},
			 new TextModel() { Text = "GOODBYE"}
		  },
				World = "world"
			}, "goodbye! Goodbye! GOODBYE! cruel world!");
			ShouldRender("Each1", new TextListModel() { Goodbyes = new List<TextModel>(), World = "world" }, "cruel world!");
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("EachWithIndex1", "{{#each Goodbyes}}{{@index}}. {{Text}}! {{/each}}cruel {{World}}!", _textListModel)]
		public void EachWithIndex()
		{
			ShouldRender("EachWithIndex1", new TextListModel()
			{
				Goodbyes = new List<TextModel>()
		  {
			 new TextModel() { Text = "goodbye"},
			 new TextModel() { Text = "Goodbye"},
			 new TextModel() { Text = "GOODBYE"}
		  },
				World = "world"
			}, "0. goodbye! 1. Goodbye! 2. GOODBYE! cruel world!");
		}


		[TestMethod]
		[RegisterHandlebarsTemplate("EachWithNestedIndex1", "{{#each Goodbyes}}{{@index}}. {{Text}}! {{#each ../Goodbyes}}{{@index}} {{/each}}After {{@index}} {{/each}}{{@index}}cruel {{World}}!", _textListModel)]
		public void EachWithNestedIndex()
		{
			ShouldRender("EachWithNestedIndex1", new TextListModel()
			{
				Goodbyes = new List<TextModel>()
		  {
			 new TextModel() { Text = "goodbye"},
			 new TextModel() { Text = "Goodbye"},
			 new TextModel() { Text = "GOODBYE"}
		  },
				World = "world"
			}, "0. goodbye! 0 1 2 After 0 1. Goodbye! 0 1 2 After 1 2. GOODBYE! 0 1 2 After 2 cruel world!");
			ShouldRaiseError("EachWithNestedIndex1", Compiler.HandlebarsTypeErrorKind.SpecialExpressionOutsideEachLoop);
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("EachObjectWithIndex1", "{{#each Goodbyes}}{{@index}}. {{Text}}! {{/each}}cruel {{World}}!", _multiTextModel)]
		public void EachObjectWithIndex()
		{
			ShouldRender("EachObjectWithIndex1", new MultiTextModel1()
			{
				Goodbyes = new ABCModel()
				{
					A = new TextModel() { Text = "goodbye" },
					B = new TextModel() { Text = "Goodbye" },
					C = new TextModel() { Text = "GOODBYE" }
				},
				World = "world"
			}, "0. goodbye! 1. Goodbye! 2. GOODBYE! cruel world!");
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("EachWithFirst1", "{{#each Goodbyes}}{{#if @first}}{{Text}}! {{/if}}{{/each}}cruel {{World}}!", _textListModel)]
		public void EachWithFirst()
		{
			ShouldRender("EachWithFirst1", new TextListModel()
			{
				Goodbyes = new List<TextModel>()
		  {
			 new TextModel() { Text = "goodbye"},
			 new TextModel() { Text = "Goodbye"},
			 new TextModel() { Text = "GOODBYE"}
		  },
				World = "world"
			}, "goodbye! cruel world!");
		}


		[TestMethod]
		[RegisterHandlebarsTemplate("EachWithNestedFirst1", "{{#each Goodbyes}}({{#if @first}}{{Text}}! {{/if}}{{#each ../Goodbyes}}{{#if @first}}{{Text}}!{{/if}}{{/each}}{{#if @first}} {{Text}}!{{/if}}) {{/each}}cruel {{World}}!", _textListModel)]
		public void EachWithNestedFirst()
		{
			ShouldRender("EachWithNestedFirst1", new TextListModel()
			{
				Goodbyes = new List<TextModel>()
		  {
			 new TextModel() { Text = "goodbye"},
			 new TextModel() { Text = "Goodbye"},
			 new TextModel() { Text = "GOODBYE"}
		  },
				World = "world"
			}, "(goodbye! goodbye! goodbye!) (goodbye!) (goodbye!) cruel world!");
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("EachObjectWithFirst1", "{{#each Goodbyes}}{{#if @first}}{{Text}}! {{/if}}{{/each}}cruel {{World}}!", _multiTextModel)]
		public void EachObjectWithFirst()
		{
			ShouldRender("EachObjectWithFirst1", new MultiTextModel1()
			{
				Goodbyes = new ABCModel()
				{
					A = new TextModel() { Text = "goodbye" },
					B = new TextModel() { Text = "Goodbye" },
					C = new TextModel() { Text = "GOODBYE" }
				},
				World = "world"
			}, "goodbye! cruel world!");
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("EachWithLast1", "{{#each Goodbyes}}{{#if @last}}{{Text}}! {{/if}}{{/each}}cruel {{World}}!", _textListModel)]
		public void EachWithLast()
		{
			ShouldRender("EachWithLast1", new TextListModel()
			{
				Goodbyes = new List<TextModel>()
		  {
			 new TextModel() { Text = "goodbye"},
			 new TextModel() { Text = "Goodbye"},
			 new TextModel() { Text = "GOODBYE"}
		  },
				World = "world"
			}, "GOODBYE! cruel world!");
		}


		[TestMethod]
		[RegisterHandlebarsTemplate("EachWithNestedLast1", "{{#each Goodbyes}}({{#if @last}}{{Text}}! {{/if}}{{#each ../Goodbyes}}{{#if @last}}{{Text}}!{{/if}}{{/each}}{{#if @last}} {{Text}}!{{/if}}) {{/each}}cruel {{World}}!", _textListModel)]
		public void EachWithNestedLast()
		{
			ShouldRender("EachWithNestedLast1", new TextListModel()
			{
				Goodbyes = new List<TextModel>()
		  {
			 new TextModel() { Text = "goodbye"},
			 new TextModel() { Text = "Goodbye"},
			 new TextModel() { Text = "GOODBYE"}
		  },
				World = "world"
			}, "(GOODBYE!) (GOODBYE!) (GOODBYE! GOODBYE! GOODBYE!) cruel world!");
		}

		[TestMethod]
		[RegisterHandlebarsTemplate("EachObjectWithLast1", "{{#each Goodbyes}}{{#if @last}}{{Text}}! {{/if}}{{/each}}cruel {{World}}!", _multiTextModel2)]
		public void EachObjectWithLast()
		{
			ShouldRender("EachObjectWithLast1", new MultiTextModel2()
			{
				Goodbyes = new FooBarModel()
				{
					Foo = new TextModel() { Text = "goodbye" },
					Bar = new TextModel() { Text = "Goodbye" },
				},
				World = "world"
			}, "Goodbye! cruel world!");
		}

		[CompiledHandlebarsHelperMethod]
		public static string DetectDataInsideEach(string date) => string.IsNullOrEmpty(date) ? string.Empty : "!";

		[TestMethod]
		[RegisterHandlebarsTemplate("EachDataPassedToHelpers1", "{{#each Letters}}{{this}}{{DetectDataInsideEach}}{{/each}}", _letterListModel)]
		public void EachDataPassedToHelpers()
		{
			ShouldRender("EachDataPassedToHelpers1", new LetterListModel() { Letters = new List<string>() { "a", "b", "c" } }, "a!b!c!");
		}

		private class CompiledHandlebarsHelperMethodAttribute : Attribute { }

	}
}
