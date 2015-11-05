using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;
using Microsoft.CodeAnalysis;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal class FirstExpression : Expression
  {
    internal override Context Evaluate(CompilationState state)
    {
      return new Context($"first{state.LoopLevel}", state.Introspector.GetBoolTypeSymbol());
    }
  }

  internal class LastExpression : Expression
  {
    internal override Context Evaluate(CompilationState state)
    {
      return new Context($"last{state.LoopLevel}", state.Introspector.GetBoolTypeSymbol());
    }
  }

  internal class IndexExpression : Expression
  {

    internal override Context Evaluate(CompilationState state)
    {
      return new Context($"index{state.LoopLevel}", state.Introspector.GetIntTypeSymbol());
    }
  }


}
