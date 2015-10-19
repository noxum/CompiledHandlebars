using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.Visitors;
using CompiledHandlebars.Compiler.AST.Expressions;

namespace CompiledHandlebars.Compiler.AST
{
  internal class PartialCall : ASTElementBase
  {
    internal readonly MemberExpression _Member;
    internal readonly string _TemplateName;


    public PartialCall(string templateName, MemberExpression member, int line, int column) : base(line, column)
    {
      _Member = member;
      _TemplateName = templateName;
    }


    internal override void Accept(IASTVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
