﻿using CompiledHandlebars.Compiler.Introspection;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal abstract class Expression
  {
    internal abstract Context EvaluateToContext(CompilationState state);

    internal abstract string EvaluateToString(CompilationState state);
  }
}
