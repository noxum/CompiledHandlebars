using CompiledHandlebars.Compiler.AST.Expressions;
using CompiledHandlebars.Compiler.Visitors;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler.AST
{

  internal class StaticHandlebarsTemplate : HandlebarsTemplate
  {
    private readonly MarkupLiteral _markupLiteral;
    internal StaticHandlebarsTemplate(MarkupLiteral markupLiteral, IList<HandlebarsSyntaxError> parseErrors) : base(parseErrors)
    {
      _markupLiteral = markupLiteral;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.VisitEnter(this);
      visitor.Visit(_markupLiteral);
      visitor.VisitLeave(this);
    }
  }
 
  internal class HandlebarsLayout : HandlebarsTemplate
  {
    private IList<ASTElementBase> _preItems;
    private IList<ASTElementBase> _postItems;

    internal HandlebarsLayout(IList<ASTElementBase> preItems, IList<ASTElementBase> postItems, NamespaceOrTypeName modelFullyQualifiedName, IList<HandlebarsSyntaxError> parseErrors)
      :base(null, modelFullyQualifiedName, parseErrors)
    {
      _preItems = preItems;
      _postItems = postItems;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.VisitEnter(this);
      foreach (var item in _preItems)
        item.Accept(visitor);
      visitor.VisitRenderBody(this);
      foreach (var item in _postItems)
        item.Accept(visitor);
      visitor.VisitLeave(this);
    }
  }

  internal class LayoutedHandlebarsTemplate : HandlebarsTemplate
  {
    internal readonly string LayoutName;

    internal LayoutedHandlebarsTemplate(string layout, IList<ASTElementBase> items, NamespaceOrTypeName modelFullyQualifiedName, IList<HandlebarsSyntaxError> parseErrors)
      :base(items, modelFullyQualifiedName, parseErrors)
    {
      LayoutName = layout;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.VisitEnter(this);
      foreach (var item in _items)
        item.Accept(visitor);
      visitor.VisitLeave(this);
    }
  }

  internal class HandlebarsTemplate
  {
    internal readonly IList<HandlebarsSyntaxError> ParseErrors;
    internal NamespaceOrTypeName ModelFullyQualifiedName { get; set; }
    internal string Name { get; set; }
    internal string Namespace { get; set; }
    protected IList<ASTElementBase> _items { get; set; }

    internal HandlebarsTemplate(IList<ASTElementBase> items, NamespaceOrTypeName modelFullyQualifiedName, IList<HandlebarsSyntaxError> parseErrors)
    {
      ModelFullyQualifiedName = modelFullyQualifiedName;
      ParseErrors = parseErrors;
      _items = items;
    }

    internal HandlebarsTemplate(IList<HandlebarsSyntaxError> parseErrors)
    {
      ParseErrors = parseErrors;
    }

    //TODO: Not clean. Build a NamespaceUtility that can handle namespaces correctly. See also: Namespace operations in RoslynIntrospector
    internal bool IsSelfReferencingPartial(string partial)
    {
      return string.Concat(Namespace, ".", Name).EndsWith(partial);
    }

    internal virtual void Accept(IASTVisitor visitor)
    {
      visitor.VisitEnter(this);
      foreach (var item in _items)
        item.Accept(visitor);
      visitor.VisitLeave(this);
    }

  }
}
