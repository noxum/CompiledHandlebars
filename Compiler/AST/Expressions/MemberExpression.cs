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

    internal override string Evaluate(Stack<Context> contextStack)
    {
      //Copy Stack as identifier elements manipulate (push, pop)
      var cpContextStack = new Stack<Context>();
      cpContextStack = new Stack<Context>(contextStack);
      return Path.Evaluate(contextStack).FullPath;
    }
  }
}
