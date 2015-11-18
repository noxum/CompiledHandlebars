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
    public List<BenchmarkCaseModel> Cases { get; set; } = new List<BenchmarkCaseModel>();
  }
  public class VMInformation
  {
    public int ProcessorCount { get; set; }
    public string OSVersion { get; set; }
    public string RuntimeVersion { get; set; }
    public string Location { get; set; }
  }

  public class BenchmarkCaseModel
  {
    public string Name { get; set; }
    public string TemplateContents { get; set; }
    public string ModelJson { get; set; }
    public List<Measurement> Items { get; set; } = new List<Measurement>();
    
    public class Measurement
    {
      public TimeSpan Duration { get; set; }
      public long Throughput { get; set; }
    }
  }



}
