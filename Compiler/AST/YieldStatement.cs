using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.Visitors;
using CompiledHandlebars.Compiler.AST.Expressions;

namespace CompiledHandlebars.Compiler.AST
{
  internal class YieldStatement : ASTElementBase
  {
    internal readonly MemberExpression Member;

    internal YieldStatement(MemberExpression member, int line, int column) : base(line, column)
    {
      Member = member;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
