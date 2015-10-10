using CompiledHandlebars.Compiler.Visitors;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler.AST
{

  internal class HandlebarsTemplate
  {
    internal string Name { get; set; }
    internal string Namespace { get; set; }
    protected IList<ASTElementBase> _items { get; set; }

    internal HandlebarsTemplate(IList<ASTElementBase> items)
    {
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
