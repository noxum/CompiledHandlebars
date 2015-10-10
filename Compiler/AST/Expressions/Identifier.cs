using CompiledHandlebars.Compiler.Introspection;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal class Identifier : IdentifierElement
  {
    private readonly string _value;

    internal Identifier(string value, IdentifierElement next) : base(next)
    {
      _value = value;
    }

    internal override Context Evaluate(Stack<Context> contextStack)
    {
      //Add the Identifier to the current context
      var identifierContext = new Context() { FullPath = string.Join(".", contextStack.Peek().FullPath, _value) };
      if (_next == null)
        //Last element => return IdentifierContext
        return identifierContext;
      else
      {
        //Push the identifier on the contextStack and keep going
        contextStack.Push(identifierContext);
        return _next.Evaluate(contextStack);
      }
    }
  }

  internal class PathUp : IdentifierElement
  {
    internal PathUp(IdentifierElement next) : base(next) { }

    internal override Context Evaluate(Stack<Context> contextStack)
    {
      contextStack.Pop();
      return _next.Evaluate(contextStack);
    }
  }


  /// <summary>
  /// Represents an identifier inside Handlebars as a linked list of IdentifierElements
  /// IdentifierElements are either
  ///   Identifier 
  ///   PathUp ("../")
  /// 
  ///Identifier seperators (".","/") can be ignored as their semantic value is represented as seperated Identifier objects
  /// 
  /// ../A.B => PathUp->Identifier(A)->Identifier(B)
  /// 
  /// </summary>
  internal abstract class IdentifierElement
  {
    protected readonly IdentifierElement _next;

    internal IdentifierElement(IdentifierElement next)
    {
      _next = next;
    }

    internal abstract Context Evaluate(Stack<Context> contextStack);

  }
}
