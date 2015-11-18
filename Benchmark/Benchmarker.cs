using CompiledHandlebars.Benchmark.MeasurementModels;
using CompiledHandlebars.Benchmark.Templates;
using CompiledHandlebars.Benchmark.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CompiledHandlebars.Benchmark
{
  public static class Benchmarker
  {
    public static BenchmarkModel CreateFullBenchmark()
    {
      var result = new BenchmarkModel();
      result.ExecutionDateStart = DateTime.UtcNow;
      result.VM = new VMInformation()
      {
        RuntimeVersion = Environment.Version.ToString(),
        OSVersion = Environment.OSVersion.ToString(),
        ProcessorCount = Environment.ProcessorCount        
      };
      result.Cases.Add(RunBenchmarkCase(PrepareComplexBenchmarkCase(), "complex"));
      result.Cases.Add(RunBenchmarkCase(PrepareArrayEachBenchmarkCase(), "arrayeach"));
      result.Cases.Add(RunBenchmarkCase(PrepareDataBenchmarkCase(), "data"));
      result.Cases.Add(RunBenchmarkCase(PrepareDepth1BenchmarkCase(), "depth1"));
      result.Cases.Add(RunBenchmarkCase(PrepareDepth2BenchmarkCase(), "depth2"));
      result.Cases.Add(RunBenchmarkCase(Templates.String.Render, "string"));
      result.Cases.Add(RunBenchmarkCase(PreparePartialBenchmarkCase(), "partial"));
      result.Cases.Add(RunBenchmarkCase(PreparePartialRecursionBenchmarkCase(), "partial-recursion"));
      result.Cases.Add(RunBenchmarkCase(PreparePathsBenchmarkCase(), "paths"));
      result.Cases.Add(RunBenchmarkCase(PrepareVariablesBenchmarkCase(), "variables"));
      Thread.Sleep(TimeSpan.FromSeconds(30));//Sleep thirty seconds then rerun
      result.Cases.Add(RunBenchmarkCase(PrepareComplexBenchmarkCase(), "complex"));
      result.Cases.Add(RunBenchmarkCase(PrepareArrayEachBenchmarkCase(), "arrayeach"));
      result.Cases.Add(RunBenchmarkCase(PrepareDataBenchmarkCase(), "data"));
      result.Cases.Add(RunBenchmarkCase(PrepareDepth1BenchmarkCase(), "depth1"));
      result.Cases.Add(RunBenchmarkCase(PrepareDepth2BenchmarkCase(), "depth2"));
      result.Cases.Add(RunBenchmarkCase(Templates.String.Render, "string"));
      result.Cases.Add(RunBenchmarkCase(PreparePartialBenchmarkCase(), "partial"));
      result.Cases.Add(RunBenchmarkCase(PreparePartialRecursionBenchmarkCase(), "partial-recursion"));
      result.Cases.Add(RunBenchmarkCase(PreparePathsBenchmarkCase(), "paths"));
      result.Cases.Add(RunBenchmarkCase(PrepareVariablesBenchmarkCase(), "variables"));
      result.ExecutionDateStop = DateTime.UtcNow;
      result.Summary = new BenchmarkSummary();
      foreach(var benchCase in result.Cases.GroupBy(x => x.Name))
      {// Evaluation
        result.Summary.Items.Add(benchCase.Key, EvaluateCases(benchCase));
      }
      return result;
    }

    private static BenchmarkSummary.BenchmarkSummaryItem EvaluateCases(IEnumerable<BenchmarkCaseModel> cases)
    {
      var summaryItem = new BenchmarkSummary.BenchmarkSummaryItem();
      var measurementList = cases.SelectMany(x => x.Items).OrderBy(x => x.Throughput).ToList();
      summaryItem.Name = cases.First().Name;
      summaryItem.SampleSize = measurementList.Count;
      summaryItem.AverageThroughput = measurementList.Average(x => x.Throughput);
      summaryItem.MedianThroughput = measurementList[measurementList.Count / 2].Throughput;
      summaryItem.StandardDeviation = Math.Sqrt(measurementList.Sum(x => Math.Pow((x.Throughput - summaryItem.AverageThroughput), 2)) / summaryItem.SampleSize);
      return summaryItem;
    }

    private static BenchmarkCaseModel RunBenchmarkCase<TViewModel>(Tuple<TViewModel, Func<TViewModel, string>> values, string name)
    {
      var result = new BenchmarkCaseModel();
      result.Name = name;
      //One dryrun
      var timeSpan = TimeSpan.FromMilliseconds(2000);
      Measure(values.Item2, values.Item1, TimeSpan.FromSeconds(1), name);
      for(int i = 0; i < 5;i++)
        result.Items.Add(Measure(values.Item2, values.Item1, timeSpan, name));
      return result;
    }

    private static BenchmarkCaseModel RunBenchmarkCase(Func<string> renderMethod, string name)
    {
      var result = new BenchmarkCaseModel();
      result.Name = name;
      var timeSpan = TimeSpan.FromMilliseconds(2000);
      //One dryrun
      Measure(renderMethod, TimeSpan.FromSeconds(1), name);
      for (int i = 0; i < 5; i++)
        result.Items.Add(Measure(renderMethod, timeSpan, name));
      return result;
    }

    private static Tuple<ComplexModel, Func<ComplexModel, string>> PrepareComplexBenchmarkCase()
    {
      return new Tuple<ComplexModel, Func<ComplexModel, string>>
      (
        new ComplexModel()
        {
          HasItems = true,
          Header = "Colors",
          Items = new List<ComplexModel.ItemModel>()
          {
            new ComplexModel.ItemModel() { Current = true, Name = "red", Url = "#Red" },
            new ComplexModel.ItemModel() { Current = true, Name = "green", Url = "#Green" },
            new ComplexModel.ItemModel() { Current = true, Name = "blue", Url = "#Blue" }
          }
        },
        Complex.Render
      );
    }

    private static Tuple<ArrayEachModel, Func<ArrayEachModel, string>> PrepareArrayEachBenchmarkCase()
    {
      return new Tuple<ArrayEachModel, Func<ArrayEachModel, string>>
      (
        new ArrayEachModel()
        {
          Names = new List<ArrayEachModel.NameModel>()
          {
            new ArrayEachModel.NameModel() { Name = "Moe" },
            new ArrayEachModel.NameModel() { Name = "Larry" },
            new ArrayEachModel.NameModel() { Name = "Curly" },
            new ArrayEachModel.NameModel() { Name = "Shemp" }
          }
        },
        ArrayEach.Render
      );
    }

    private static Tuple<ArrayEachModel, Func<ArrayEachModel, string>> PrepareDataBenchmarkCase()
    {
      return new Tuple<ArrayEachModel, Func<ArrayEachModel, string>>
      (
        new ArrayEachModel()
        {
         Names = new List<ArrayEachModel.NameModel>()
          {
            new ArrayEachModel.NameModel() { Name = "Moe" },
            new ArrayEachModel.NameModel() { Name = "Larry" },
            new ArrayEachModel.NameModel() { Name = "Curly" },
            new ArrayEachModel.NameModel() { Name = "Shemp" }
          }
        },
        Data.Render
      );
    }

    private static Tuple<Depth1Model, Func<Depth1Model, string>> PrepareDepth1BenchmarkCase()
    {
      return new Tuple<Depth1Model, Func<Depth1Model, string>>
      (
        new Depth1Model()
        {
          Foo = "bar",
          Names = new List<Depth1Model.NameModel>()
          {
            new Depth1Model.NameModel() { Name = "Moe" },
            new Depth1Model.NameModel() { Name = "Larry" },
            new Depth1Model.NameModel() { Name = "Curly" },
            new Depth1Model.NameModel() { Name = "Shemp" }
          }
        },
        Depth1.Render
      );
    }

    private static Tuple<Depth2Model, Func<Depth2Model, string>> PrepareDepth2BenchmarkCase()
    {
      return new Tuple<Depth2Model, Func<Depth2Model, string>>(
        new Depth2Model()
        {
          Foo = "bar",
          Names = new List<Depth2Model.FooNameModel>()
          {
            new Depth2Model.FooNameModel() {Bat = "foo", Name = new List<string>() { "Moe" } },
            new Depth2Model.FooNameModel() {Bat = "foo", Name = new List<string>() { "Larry" } },
            new Depth2Model.FooNameModel() {Bat = "foo", Name = new List<string>() { "Curly" } },
            new Depth2Model.FooNameModel() {Bat = "foo", Name = new List<string>() { "Shemp" } },
          }
        }, 
        Depth2.Render
      );
    }

    private static Tuple<PartialModel, Func<PartialModel, string>> PreparePartialBenchmarkCase()
    {
      return new Tuple<PartialModel, Func<PartialModel, string>>(
        new PartialModel()
        {
          Peeps = new List<VariablesModel>()
          {
            new VariablesModel() { Count = 15, Name = "Moe" },
            new VariablesModel() { Count = 15, Name = "Lary" },
            new VariablesModel() { Count = 15, Name = "Curly" }
          }
        },
        Partial.Render
      );      
    }

    private static Tuple<PartialRecursionModel, Func<PartialRecursionModel, string>> PreparePartialRecursionBenchmarkCase()
    {
      return new Tuple<PartialRecursionModel, Func<PartialRecursionModel, string>>(
        new PartialRecursionModel()
        {
          Name = "1",
          Kids = new List<PartialRecursionModel>()
          {
            new PartialRecursionModel()
            {
              Name = "1.1",
              Kids = new List<PartialRecursionModel>()
              {
                new PartialRecursionModel()
                {
                  Name = "1.1.1"                  
                }
              }
            }
          }
        },
        Recursion.Render
      );
    }

    private static Tuple<PathsModel, Func<PathsModel, string>> PreparePathsBenchmarkCase()
    {
      return new Tuple<PathsModel, Func<PathsModel, string>>(
        new PathsModel()
        {
          Person = new PathsModel.PersonModel()
          {
            Age = 45,
            Name = new PathsModel.PersonModel.NameModel() {  Bar = new PathsModel.PersonModel.NameModel.BarModel() {  Baz = "Larry"} }
          }
        }
        ,
        Paths.Render
        );
    }

    private static Tuple<VariablesModel, Func<VariablesModel, string>> PrepareVariablesBenchmarkCase()
    {
      return new Tuple<VariablesModel, Func<VariablesModel, string>>(
        new VariablesModel()
        {
          Name = "Mick",
          Count = 30
        }
        ,
        Variables.Render
        );
    }

    private static BenchmarkCaseModel.Measurement Measure<TViewModel>(Func<TViewModel, string> RenderMethod, TViewModel viewModel, TimeSpan duration, string name)
    {
      var sw = new Stopwatch();
      long counter = 0;
      while (sw.ElapsedMilliseconds < duration.TotalMilliseconds)
      {
        sw.Start();
        RenderMethod(viewModel);
        sw.Stop();
        counter++;
      }
      return new BenchmarkCaseModel.Measurement()
      {
        Duration = duration,
        Throughput = counter / (sw.ElapsedMilliseconds)
      };
    }

    private static BenchmarkCaseModel.Measurement Measure(Func<string> RenderMethod, TimeSpan duration, string name)
    {
      var sw = new Stopwatch();
      long counter = 0;
      while (sw.ElapsedMilliseconds < duration.TotalMilliseconds)
      {
        sw.Start();
        RenderMethod();
        sw.Stop();
        counter++;
      }
      return new BenchmarkCaseModel.Measurement()
      {
        Duration = duration,
        Throughput = counter / (sw.ElapsedMilliseconds)
      };
    }

  }
}
