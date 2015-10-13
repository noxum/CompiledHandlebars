using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler;

namespace CompiledHandlebars.Cli
{
  class Program
  {
    static void Main(string[] args)
    {
      HbsCompiler.Compile("{{model Test}} test {{Test}}", string.Empty, string.Empty, null);
    }
  }
}
