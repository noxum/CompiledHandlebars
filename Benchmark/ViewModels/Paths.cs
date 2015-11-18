using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Benchmark.ViewModels
{
  public class PathsModel
  {
    public PersonModel Person { get; set; }
    public class PersonModel
    {
      public int Age { get; set; }
      public NameModel Name { get; set; }
      public class NameModel
      {
        public BarModel Bar { get; set; }
        public class BarModel
        {
          public string Baz { get; set; }
        }
      }
    }
  }
}
