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

    internal readonly Expression Member;
    internal readonly IfType QueryType;
    internal readonly bool HasElseBlock;

    internal IfBlock(Expression member, IfType type, IList<ASTElementBase> children, int line, int column) 
                    : base(children, line, column)
    {
      QueryType = type;
      HasElseBlock = false;
      Member = member;
    }

    internal IfBlock(Expression member, IfType type, IList<ASTElementBase> elseBlock, IList<ASTElementBase> children, int line, int column) 
                    : base(children, line, column)
    {
      QueryType = type;
      _elseBlock = elseBlock;
      HasElseBlock = true;
      Member = member;
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
  }

  internal enum IfType { If, Unless }
}
