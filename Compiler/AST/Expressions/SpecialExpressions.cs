using System;
using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;
using Microsoft.CodeAnalysis;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  
  internal abstract class SpecialExpression : Expression
  {
    protected bool InsideEachLoopCheck(CompilationState state)
    {
      if (state.LoopLevel > 0)
        return true;
      state.AddTypeError("SpecialExpressions can only exist inside EachBlocks", HandlebarsTypeErrorKind.SpecialExpressionOutsideEachLoop);
      return false;
    }
  }

  internal class FirstExpression : SpecialExpression
  {
    internal override bool TryEvaluate(CompilationState state, out Context context)
    {     
      context = new Context($"first{state.LoopLevel}", state.Introspector.GetBoolTypeSymbol());
      return InsideEachLoopCheck(state);
    }
  }

  internal class LastExpression : SpecialExpression
  {

    internal override bool TryEvaluate(CompilationState state, out Context context)
    {
      context = new Context($"last{state.LoopLevel}", state.Introspector.GetBoolTypeSymbol());
      return InsideEachLoopCheck(state);      
    }
  }

  internal class IndexExpression : SpecialExpression
  {   

    internal override bool TryEvaluate(CompilationState state, out Context context)
    {
      context = new Context($"index{state.LoopLevel}", state.Introspector.GetIntTypeSymbol());
      return InsideEachLoopCheck(state);
    }
  }


}
