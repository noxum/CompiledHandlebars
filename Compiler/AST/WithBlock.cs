using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.Visitors;
using CompiledHandlebars.Compiler.AST.Expressions;

namespace CompiledHandlebars.Compiler.AST
{
  internal class WithBlock : ASTNode
  {

    internal readonly Expression Expr;

    internal WithBlock(Expression expr, IList<ASTElementBase> children, int line, int column) : base(children, line, column)
    {
      Expr = expr;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.VisitEnter(this);
      foreach (var child in _children)
        child.Accept(visitor);
      visitor.VisitLeave(this);
    }

    internal override bool HasExpressionOnLoopLevel<T>()
    {
      if (Expr is T)
        return true;
      return _children.Any(x => x.HasExpressionOnLoopLevel<T>());
    }
  }
}
