using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler
{

  public class HandlebarsTypeError : HandlebarsException {
    public HandlebarsTypeError(string message) : base(message) { }
  }

  public class HandlebarsSyntaxError : HandlebarsException {
    public HandlebarsSyntaxError(string message) : base(message) { }
  }

  public abstract class HandlebarsException : Exception
  {
    public HandlebarsException(string message) : base(message) { }
  }
}
