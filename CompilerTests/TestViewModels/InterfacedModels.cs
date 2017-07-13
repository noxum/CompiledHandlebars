using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.TestViewModels
{
	public interface IBase
	{
		int AnotherInt { get; set; }
		string AnotherString { get; set; }
		List<string> AnotherList { get; set; }
		DummyClass AnotherClass { get; set; }
	}

	public interface IDerived : IBase
	{
		double AnotherDouble { get; set; }
	}


	public class ImplementsDerivedModel : IDerived
	{
		public double AnotherDouble { get; set; }
		public int AnotherInt { get; set; }
		public List<string> AnotherList { get; set; }
		public string AnotherString { get; set; }
		public DummyClass AnotherClass { get; set; }
	}

	public class ImplementsBaseModel : IBase
	{
		public int AnotherInt { get; set; }
		public List<string> AnotherList { get; set; }
		public string AnotherString { get; set; }
		public DummyClass AnotherClass { get; set; }
	}

	public class DummyClass
	{
		public string DummyString { get; set; }
	}

	public static class DerivedModelFactory
	{
		public static ImplementsDerivedModel CreateFullDerivedModel()
		{
			var model = new ImplementsDerivedModel();
			model.AnotherInt = 10;
			model.AnotherString = "nothing";
			model.AnotherList = new List<string>();
			model.AnotherList.Add("firstString");
			model.AnotherList.Add("secondString");
			model.AnotherDouble = 5.5;
			model.AnotherClass = new DummyClass();
			model.AnotherClass.DummyString = "justAnotherString";
			return model;
		}
	}

	public static class BaseModelFactory
	{
		public static ImplementsBaseModel CreateFullBaseModel()
		{
			var model = new ImplementsBaseModel();
			model.AnotherInt = 20;
			model.AnotherString = "everything";
			model.AnotherList = new List<string>();
			model.AnotherList.Add("thisString");
			model.AnotherList.Add("thatString");
			model.AnotherClass = new DummyClass();
			model.AnotherClass.DummyString = "justAnotherString";
			return model;
		}
	}
}
