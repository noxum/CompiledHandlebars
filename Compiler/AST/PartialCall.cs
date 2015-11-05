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
    internal readonly Expression Expr;
    internal readonly string TemplateName;    

    public PartialCall(string templateName, Expression member, int line, int column) : base(line, column)
    {     
      Expr = member;
      TemplateName = templateName;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.Visit(this);
    }

    internal override bool HasExpressionOnLoopLevel<T>()
    {
      return (Expr is T);
    }
  }
}
