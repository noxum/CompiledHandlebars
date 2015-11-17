using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Benchmark.ViewModels
{
  public class ComplexModel
  {
    public string Header { get; set; }
    public bool HasItems { get; set; }
    public List<ItemModel> Items { get; set; }
    
    public class ItemModel
    {
      public string Name { get; set; }
      public bool Current { get; set; }
      public string Url { get; set; }
    }
  }
}
