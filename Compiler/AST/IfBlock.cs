using CompiledHandlebars.Compiler.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.Visitors;

namespace CompiledHandlebars.Compiler.AST
{
  internal class IfBlock : ASTNode
  {
    private readonly IList<ASTElementBase> _elseBlock;

    internal readonly Expression Expr;
    internal readonly IfType QueryType;
    internal readonly bool HasElseBlock;

    internal IfBlock(Expression member, IfType type, IList<ASTElementBase> children, int line, int column) 
                    : base(children, line, column)
    {
      QueryType = type;
      HasElseBlock = false;
      Expr = member;
    }

    internal IfBlock(Expression expr, IfType type, IList<ASTElementBase> elseBlock, IList<ASTElementBase> children, int line, int column) 
                    : base(children, line, column)
    {
      QueryType = type;
      _elseBlock = elseBlock;
      HasElseBlock = true;
      Expr = expr;
    }


    internal override void Accept(IASTVisitor visitor)
    {
      visitor.VisitEnter(this);
      foreach (var child in _children)
        child.Accept(visitor);
      if(HasElseBlock)
      {
        visitor.VisitElse(this);
        foreach(var ele in _elseBlock)
        {
          ele.Accept(visitor);
        }
      }
      visitor.VisitLeave(this);
    }

    internal override bool HasExpression<T>(bool includeChildren = false)
    {
      if (Expr is T)
        return true;
      if (includeChildren)
      {
        if (HasElseBlock)
          return _elseBlock.Any(x => x.HasExpression<T>(includeChildren)) || _children.Any(x => x.HasExpression<T>(includeChildren));
        else
          return _children.Any(x => x.HasExpression<T>(includeChildren));
      }
      return false;
    }
  }

  internal enum IfType { If, Unless }
}
