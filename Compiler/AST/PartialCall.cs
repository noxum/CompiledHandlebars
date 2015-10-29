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
    internal readonly Expression _Member;
    internal readonly string _TemplateName;
    internal readonly bool _HasMember;

    public PartialCall(string templateName, Expression member, int line, int column) : base(line, column)
    {
      _HasMember = true;
      _Member = member;
      _TemplateName = templateName;
    }

    public PartialCall(string templateName, int line, int column) : base(line, column)
    {
      _HasMember = false;
      _TemplateName = templateName;
    }


    internal override void Accept(IASTVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
