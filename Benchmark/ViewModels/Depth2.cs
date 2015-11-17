using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Benchmark.ViewModels
{
  public class Depth2Model
  {
    public string Foo { get; set; }
    public List<FooNameModel> Names { get; set; }
    public class FooNameModel
    {
      public string Bat { get; set; }
      public List<string> Name { get; set; }
    }
  }
}
