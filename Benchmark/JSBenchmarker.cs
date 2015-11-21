using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EdgeJs;


namespace CompiledHandlebars.Benchmark
{
  public static class JSBenchmarker
  {
    public static async Task<string> Run()
    {
      var increment = Edge.Func(@"
           var current = 0;

            return function (data, callback) {
                current += data;
                callback(null, current);
            }
      ");
      return (await increment(5)).ToString();
    }
  }
}
