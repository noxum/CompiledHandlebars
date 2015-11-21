using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Benchmark.MeasurementModels
{
  public class BenchmarkModel
  {
    public string CommitHash { get; set; }
    public DateTime ExecutionDateStart { get; set; }
    public DateTime ExecutionDateStop { get; set; }
    public VMInformation VM { get;set;}
    public BenchmarkSummary Summary { get; set; }
    public List<BenchmarkCase> Cases { get; set; } = new List<BenchmarkCase>();
  }
  public class VMInformation
  {
    public double CPULoad { get; set; }
    public int ProcessorCount { get; set; }
    public string OSVersion { get; set; }
    public string RuntimeVersion { get; set; }
    public string MachineName { get; set; }
    public List<string> RunningServices { get; set; }
  }

  public class BenchmarkCase
  {
    public string Name { get; set; }
    public string TemplateContents { get; set; }
    public string ModelJson { get; set; }
    public List<Measurement> Items { get; set; } = new List<Measurement>();
    
    public class Measurement
    {
      public long RenderCount { get; set; }
      public double Throughput { get; set; }
    }
  }



}
