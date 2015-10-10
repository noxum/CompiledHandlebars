using CompiledHandlebars.Compiler.AST.Expressions;
using CompiledHandlebars.Compiler.Visitors;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler.AST
{

  public class HandlebarsTemplate
  {
    internal MemberExpression Model { get; set; }
    internal string Name { get; set; }
    internal string Namespace { get; set; }
    private IList<ASTElementBase> _items { get; set; }

    internal HandlebarsTemplate(IList<ASTElementBase> items, MemberExpression model)
    {
      Model = model;
      _items = items;
    }

    internal void Accept(IASTVisitor visitor)
    {
      visitor.VisitEnter(this);
      foreach (var item in _items)
        item.Accept(visitor);
      visitor.VisitLeave(this);
    }

  }
}
