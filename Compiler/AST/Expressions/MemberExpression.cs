using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.Introspection;

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
