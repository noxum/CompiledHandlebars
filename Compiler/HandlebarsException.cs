using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler
{

  public class HandlebarsTypeError : HandlebarsException {
    public HandlebarsTypeError(string message, int line, int column) : base(message, line, column) { }
  }

  public class HandlebarsSyntaxError : HandlebarsException {
    public HandlebarsSyntaxError(string message, int line, int column) : base(message, line, column) { }
  }

  public abstract class HandlebarsException : Exception
  {
    public int Line { get; set; }
    public int Column { get; set; }
    public HandlebarsException(string message, int line, int column) : base(string.Format("Line: {0}; Column {1}: {2}", line, column, message))
    {
      Line = line;
      Column = column;
    }
  }
}
