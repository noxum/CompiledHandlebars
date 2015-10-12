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
    internal readonly MemberExpression Member;

    internal WithBlock(MemberExpression member, IList<ASTElementBase> children, int line, int column) : base(children, line, column)
    {
      Member = member;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.VisitEnter(this);
      foreach (var child in _children)
        child.Accept(visitor);
      visitor.VisitLeave(this);
    }
  }
}
