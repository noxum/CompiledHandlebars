using System;
using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;
using Microsoft.CodeAnalysis;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal class FirstExpression : Expression
  {
    internal override bool TryEvaluate(CompilationState state, out Context context)
    {
      context = new Context($"first{state.LoopLevel}", state.Introspector.GetBoolTypeSymbol());
      return true;
    }
  }

  internal class LastExpression : Expression
  {

    internal override bool TryEvaluate(CompilationState state, out Context context)
    {
      context = new Context($"last{state.LoopLevel}", state.Introspector.GetBoolTypeSymbol());
      return true;
    }
  }

  internal class IndexExpression : Expression
  {   

    internal override bool TryEvaluate(CompilationState state, out Context context)
    {
      context = new Context($"index{state.LoopLevel}", state.Introspector.GetIntTypeSymbol());
      return true;
    }
  }


}
