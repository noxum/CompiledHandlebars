using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal class FirstExpression : Expression
  {
    internal override Context Evaluate(CompilationState state)
    {
      return new Context($"first{state.loopLevel}", null);
    }
  }

  internal class LastExpression : Expression
  {
    internal override Context Evaluate(CompilationState state)
    {
      return new Context($"last{state.loopLevel}", null);
    }
  }

  internal class IndexExpression : Expression
  {
    internal override Context Evaluate(CompilationState state)
    {
      return new Context($"index{state.loopLevel}", null);
    }

  }
}
