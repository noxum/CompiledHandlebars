using CompiledHandlebars.Compiler.AST.Expressions;
using CompiledHandlebars.Compiler.Visitors;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler.AST
{

  public class HandlebarsTemplate
  {
    internal readonly IList<HandlebarsSyntaxError> _ParseErrors;
    internal MemberExpression Model { get; set; }
    internal string Name { get; set; }
    internal string Namespace { get; set; }
    private IList<ASTElementBase> _items { get; set; }

    internal HandlebarsTemplate(IList<ASTElementBase> items, MemberExpression model, IList<HandlebarsSyntaxError> parseErrors)
    {
      Model = model;
      _items = items;
      _ParseErrors = parseErrors;
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
