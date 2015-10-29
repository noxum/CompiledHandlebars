using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal class FirstExpressions : Expression
  {
    internal override Context Evaluate(CompilationState state)
    {
      return new Context($"first{state.loopLevel}", null);
    }
  }
}
