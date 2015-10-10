using CompiledHandlebars.Compiler.Introspection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
  internal abstract class Expression
  {
    internal abstract string Evaluate(Stack<Context> contextStack);
  }
}
