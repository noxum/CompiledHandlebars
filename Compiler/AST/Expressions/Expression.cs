using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal abstract class Expression
  {
    internal abstract bool TryEvaluate(CompilationState state, out Context context);
  }
}
