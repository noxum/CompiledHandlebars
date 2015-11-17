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
      result.Cases.Add(RunBenchmarkCase(PrepareArrayEachBenchmarkCase(), "data"));
      result.Cases.Add(RunBenchmarkCase(PrepareArrayEachBenchmarkCase(), "depth1"));
      result.Cases.Add(RunBenchmarkCase(PrepareArrayEachBenchmarkCase(), "depth2"));
      result.Cases.Add(RunBenchmarkCase(Templates.String.Render, "string"));
      result.ExecutionDateStop = DateTime.UtcNow;
      return result;
    }

    private static BenchmarkCaseModel RunBenchmarkCase<TViewModel>(Tuple<TViewModel, Func<TViewModel, string>> values, string name)
    {
      var result = new BenchmarkCaseModel();
      result.Name = name;
      result.Items.Add(Measure(values.Item2, values.Item1, TimeSpan.FromSeconds(1), name));
      result.Items.Add(Measure(values.Item2, values.Item1, TimeSpan.FromSeconds(1), name));
      result.Items.Add(Measure(values.Item2, values.Item1, TimeSpan.FromSeconds(1), name));
      result.Items.Add(Measure(values.Item2, values.Item1, TimeSpan.FromSeconds(1), name));
      result.Items.Add(Measure(values.Item2, values.Item1, TimeSpan.FromSeconds(1), name));
      return result;
    }

    private static BenchmarkCaseModel RunBenchmarkCase(Func<string> render, string name)
    {
      var result = new BenchmarkCaseModel();
      result.Name = name;
      result.Items.Add(Measure(render, TimeSpan.FromSeconds(1), name));
      result.Items.Add(Measure(render, TimeSpan.FromSeconds(1), name));
      result.Items.Add(Measure(render, TimeSpan.FromSeconds(1), name));
      result.Items.Add(Measure(render, TimeSpan.FromSeconds(1), name));
      result.Items.Add(Measure(render, TimeSpan.FromSeconds(1), name));
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
      Console.WriteLine($"Benchmaker '{name}': Duration: {duration.ToString()};\t Throughput {counter / sw.ElapsedMilliseconds}");
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
      Console.WriteLine($"Benchmaker '{name}': Duration: {duration.ToString()};\t Throughput {counter / sw.ElapsedMilliseconds}");
      return new BenchmarkCaseModel.Measurement()
      {
        Duration = duration,
        Throughput = counter / (sw.ElapsedMilliseconds)
      };
    }

  }
}
