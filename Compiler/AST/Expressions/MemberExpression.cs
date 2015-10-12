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
    internal override Context EvaluateToContext(CompilationState state)
    {
      //Copy Stack as identifier elements manipulate (push, pop)
      var cpContextStack = new Stack<Context>();
      cpContextStack = new Stack<Context>(state.ContextStack.Reverse());
      return Path.Evaluate(cpContextStack, state);
    }


    internal override string EvaluateToString(CompilationState state)
    {
      return EvaluateToContext(state).FullPath;
    }

    public override string ToString()
    {
      return Path.ToString();
    }

  }
}
