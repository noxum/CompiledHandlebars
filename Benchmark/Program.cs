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
  class Program
  {
    static void Main(string[] args)
    {

      var benchCase = Benchmarker.CreateFullBenchmark();
      Console.ReadLine();
    }       


   
  }
}
