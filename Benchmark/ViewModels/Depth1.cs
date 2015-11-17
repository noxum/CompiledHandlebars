using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Benchmark.ViewModels
{
  public class Depth1Model
  {
    public string Foo { get; set; }
    public List<NameModel> Names { get; set; }
    public class NameModel
    {
      public string Name { get; set; }
    }
  }
}
