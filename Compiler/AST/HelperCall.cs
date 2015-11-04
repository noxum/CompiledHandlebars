using CompiledHandlebars.Compiler.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.Visitors;

namespace CompiledHandlebars.Compiler.AST
{
  internal class HelperCall : ASTElementBase
  {
    internal readonly IList<Expression> Parameters;
    internal readonly string FunctionName;
    internal TokenType Type { get; set; } = TokenType.Encoded;

    internal HelperCall(string functionName, IList<Expression> parameters, int line, int column) : base(line, column)
    {
      FunctionName = functionName;
      Parameters = parameters;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.Visit(this);
    }

    internal override bool HasExpressionOnLoopLevel<T>()
    {
      return Parameters.Any(x => x is T);
    }
  }
}
