using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal class LiteralExpression : Expression
  {
    private readonly string _value;

    internal LiteralExpression(string value)
    {
      _value = value;
    }

    internal override bool TryEvaluate(CompilationState state, out Context context)
    {
      context = new Context($"\"{_value}\"", state.Introspector.GetStringTypeSymbol());
      return true;
    }
  }
}
