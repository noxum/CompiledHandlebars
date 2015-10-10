using CompiledHandlebars.Compiler.Introspection;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal class MemberExpression : Expression
  {
    public readonly IdentifierElement Path;
    internal MemberExpression(IdentifierElement path)
    {
      Path = path;
    }

    internal override string Evaluate(CompilationState state)
    {
      //Copy Stack as identifier elements manipulate (push, pop)
      var cpContextStack = new Stack<Context>();
      cpContextStack = new Stack<Context>(state.ContextStack);
      return Path.Evaluate(cpContextStack, state).FullPath;
    }

    public override string ToString()
    {
      return Path.ToString();
    }

  }
}
