using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Benchmark.ViewModels
{
  public class PartialRecursionModel
  {
    public string Name { get; set; }
    public List<PartialRecursionModel> Kids { get; set; } = new List<PartialRecursionModel>();
  }
}
