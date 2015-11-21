using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Benchmark.MeasurementModels
{
  public class BenchmarkSummary
  {
    public double AverageStandardDeviation { get; set; }
    public Dictionary<string, BenchmarkSummaryItem> Items { get; set; } = new Dictionary<string, BenchmarkSummaryItem>();
    public class BenchmarkSummaryItem
    {
      public string Name { get; set; }
      public double AverageThroughput { get; set; }
      public double MedianThroughput { get; set; }
      public int SampleSize { get; set; }
      public double StandardDeviation { get; set; }
      public double NormalizedStandardDeviation { get; set; }
    }

  }
}
