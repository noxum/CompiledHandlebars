using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal abstract class Expression
  {
    internal abstract Context Evaluate(CompilationState state);
  }
}
