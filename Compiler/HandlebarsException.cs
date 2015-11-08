using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler
{

  public class HandlebarsTypeError : HandlebarsException {
    public HandlebarsTypeErrorKind Kind { get; set; }
    public HandlebarsTypeError(string message, HandlebarsTypeErrorKind kind, int line, int column) : base(message, line, column)
    {
      Kind = kind;
    }


  }
  public enum HandlebarsTypeErrorKind { UnknownType, UnknownMember, EmptyContextStack, UnknownViewModel, UnreachableCode, CompilationFailed, UnknownPartial, UnknownHelper, UnknownLayout }

  public class HandlebarsSyntaxError : HandlebarsException {
    public HandlebarsSyntaxErrorKind Kind { get; set; }
    public HandlebarsSyntaxError(string message, HandlebarsSyntaxErrorKind kind, int line, int column) : base(message, line, column)
    {
      Kind = kind;
    }

  }
  public enum HandlebarsSyntaxErrorKind { MalformedPartialCallToken, MissingModelToken, MalformedModelToken, UnknownBlock, MalformedBlock, MissingMemberExpression, MalformedMemberExpression, UnexpectedCharacter, UnknownSpecialExpression }

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
