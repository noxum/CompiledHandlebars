using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
    internal class EqualsExpression : Expression
    {
        internal override bool TryEvaluate(CompilationState state, out Context context)
        {
            throw new NotImplementedException();
        }
    }
}
