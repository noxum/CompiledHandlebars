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
    internal readonly MemberExpression Member;
    internal readonly IfType Type;
    internal IfBlock(MemberExpression member, IfType type, IList<ASTElementBase> children, int line, int column) : base(children, line, column)
    {
      Member = member;
      Type = type;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.VisitEnter(this);
      foreach (var child in _children)
        child.Accept(visitor);
      visitor.VisitLeave(this);
    }
  }

  internal enum IfType { If, Unless }
}
