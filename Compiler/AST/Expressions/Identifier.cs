using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;
using System.Collections.Generic;
using System.Linq;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal class Identifier : IdentifierElement
  {
    private readonly string _value;

    internal Identifier(string value, IdentifierElement next) : base(next)
    {
      _value = value;
    }

    internal override Context Evaluate(Stack<Context> contextStack, CompilationState state)
    {
      //Add the Identifier to the current context
      var memberSymbol = contextStack.Any() ? contextStack.Peek().Symbol.FindMember(_value) : 
                                              state.Introspector.GetTypeSymbol(_value);      
      if (memberSymbol!=null)
      {        
        var identifierContext = new Context(string.Join(".", contextStack.Peek().FullPath, _value), memberSymbol);      
        if (_next == null)
          //Last element => return IdentifierContext
          return identifierContext;
        else
        {
          //Push the identifier on the contextStack and keep going
          contextStack.Push(identifierContext);
          return _next.Evaluate(contextStack, state);
        }
      } else
      {
        state.AddTypeError($"Could not find Member '{_value}' in Type '{contextStack.Peek().FullPath}'!");
        return contextStack.Peek();
      }
    }

    public override string ToString()
    {
      if (_next != null)
        return string.Join(".", _value, _next.ToString());
      return _value;
    }
  }

  internal class PathUp : IdentifierElement
  {
    internal PathUp(IdentifierElement next) : base(next) { }

    internal override Context Evaluate(Stack<Context> contextStack, CompilationState state)
    {
      if (!contextStack.Any())
        state.AddTypeError("Error in MemberExpression: Empty ContextStack but PathUp Element ('../')!");
      contextStack.Pop();
      return _next.Evaluate(contextStack, state);
    }

    public override string ToString()
    {
      return string.Concat("../", _next.ToString());
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

    internal abstract Context Evaluate(Stack<Context> contextStack, CompilationState state);

  }
}
