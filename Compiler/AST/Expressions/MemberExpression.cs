using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;
using System.Collections.Generic;
using System.Linq;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal class MemberExpression : Expression
  {
    public readonly IdentifierElement Path;
    internal MemberExpression(IdentifierElement path)
    {
      Path = path;
    }
    internal override Context Evaluate(CompilationState state)
    {
      //Copy Stack as identifier elements manipulate (push, pop)
      var cpContextStack = new Stack<Context>();
      cpContextStack = new Stack<Context>(state.ContextStack.Reverse());
      return Path.Evaluate(cpContextStack, state);
    }

    public override string ToString()
    {
      return Path.ToString();
    }

  }
}
