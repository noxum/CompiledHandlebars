using CompiledHandlebars.Benchmark.MeasurementModels;
using CompiledHandlebars.Benchmark.ViewModels;
using CompiledHandlebars.Benchmark.ViewModels.MeasurementModels.Templates.Templates;
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
      var services = System.ServiceProcess.ServiceController.GetServices();
      result.VM = new VMInformation()
      {
        RuntimeVersion = Environment.Version.ToString(),
        OSVersion = Environment.OSVersion.ToString(),
        ProcessorCount = Environment.ProcessorCount,
        MachineName = Environment.MachineName,
        RunningServices = services.Where(x => x.Status == System.ServiceProcess.ServiceControllerStatus.Running).Select(x => $"{x.DisplayName}|{x.ServiceName}").ToList()                              
      };
      result.Cases.AddRange(RunAllBenchmarks(100, 1000000));
      Thread.Sleep(TimeSpan.FromSeconds(30));//Sleep thirty seconds then rerun
      result.Cases.AddRange(RunAllBenchmarks(100, 1000000));
      result.ExecutionDateStop = DateTime.UtcNow;
      result.Summary = new BenchmarkSummary();
      foreach(var benchCase in result.Cases.GroupBy(x => x.Name))
      {// Evaluation
        result.Summary.Items.Add(benchCase.Key, EvaluateCases(benchCase));
      }
      result.Summary.AverageStandardDeviation = result.Summary.Items.Average(x => x.Value.NormalizedStandardDeviation);
      return result;
    }

    private static List<BenchmarkCase> RunAllBenchmarks(int runs, long renderCountPerRun)
    {
      var result = new List<BenchmarkCase>();
      result.Add(RunBenchmarkCase(PrepareComplexBenchmarkCase(), "complex", runs, renderCountPerRun));
      result.Add(RunBenchmarkCase(PrepareArrayEachBenchmarkCase(), "arrayeach", runs, renderCountPerRun));
      result.Add(RunBenchmarkCase(PrepareDataBenchmarkCase(), "data", runs, renderCountPerRun));
      result.Add(RunBenchmarkCase(PrepareDepth1BenchmarkCase(), "depth1", runs, renderCountPerRun));
      result.Add(RunBenchmarkCase(PrepareDepth2BenchmarkCase(), "depth2", runs, renderCountPerRun));
      result.Add(RunBenchmarkCase(ViewModels.MeasurementModels.Templates.Templates.String.Render, "string", runs, renderCountPerRun));
      result.Add(RunBenchmarkCase(PreparePartialBenchmarkCase(), "partial", runs, renderCountPerRun));
      result.Add(RunBenchmarkCase(PreparePartialRecursionBenchmarkCase(), "partial-recursion", runs, renderCountPerRun));
      result.Add(RunBenchmarkCase(PreparePathsBenchmarkCase(), "paths", runs, renderCountPerRun));
      result.Add(RunBenchmarkCase(PrepareVariablesBenchmarkCase(), "variables", runs, renderCountPerRun));
      return result;
    }

    private static BenchmarkSummary.BenchmarkSummaryItem EvaluateCases(IEnumerable<BenchmarkCase> cases)
    {
      var summaryItem = new BenchmarkSummary.BenchmarkSummaryItem();
      var measurementList = cases.SelectMany(x => x.Items).OrderBy(x => x.Throughput).ToList();
      summaryItem.Name = cases.First().Name;
      summaryItem.SampleSize = measurementList.Count;
      summaryItem.AverageThroughput = measurementList.Average(x => x.Throughput);
      summaryItem.MedianThroughput = measurementList[measurementList.Count / 2].Throughput;
      summaryItem.StandardDeviation = Math.Sqrt(measurementList.Sum(x => Math.Pow((x.Throughput - summaryItem.AverageThroughput), 2)) / summaryItem.SampleSize);
      summaryItem.NormalizedStandardDeviation = summaryItem.StandardDeviation / summaryItem.AverageThroughput;
      return summaryItem;
    }

    private static BenchmarkCase RunBenchmarkCase<TViewModel>(Tuple<TViewModel, Func<TViewModel, string>> values, string name, int runs, long renderCountPerRun)
    {
      Console.WriteLine($"Started case '{name}'");
      var result = new BenchmarkCase();
      result.Name = name;
      //One dryrun      
      Measure(values.Item2, values.Item1, 100000, name);
      for(int i = 0; i < runs;i++)
        result.Items.Add(Measure(values.Item2, values.Item1, renderCountPerRun, name));
      return result;
    }

    private static BenchmarkCase RunBenchmarkCase(Func<string> renderMethod, string name, int runs, long renderCountPerRun)
    {
      var result = new BenchmarkCase();
      result.Name = name;      
      //One dryrun
      Measure(renderMethod, 100000, name);
      for (int i = 0; i < runs; i++)
        result.Items.Add(Measure(renderMethod, renderCountPerRun, name));
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

    private static BenchmarkCase.Measurement Measure<TViewModel>(Func<TViewModel, string> RenderMethod, TViewModel viewModel, long renderCountPerRun, string name)
    {
      var sw = new Stopwatch();
      sw.Start();
      for (int i = 0; i < renderCountPerRun; i++)
        RenderMethod(viewModel);
      sw.Stop();
      Console.Write($"{sw.ElapsedMilliseconds}|");
      return new BenchmarkCase.Measurement()
      {
        RenderCount= renderCountPerRun,
        Throughput = renderCountPerRun / (sw.ElapsedMilliseconds)
      };
    }

    private static BenchmarkCase.Measurement Measure(Func<string> RenderMethod, long renderCount, string name)
    {
      var sw = new Stopwatch();
      sw.Start();
      for (int i = 0; i < renderCount; i++)
        RenderMethod();
      sw.Stop();
      return new BenchmarkCase.Measurement()
      {
        RenderCount = renderCount,
        Throughput = renderCount / (sw.ElapsedMilliseconds)
      };
    }

  }
}
