using CompiledHandlebars.Compiler.AST;

namespace CompiledHandlebars.Compiler.Visitors
{
  /// <summary>
  /// This is the interface for a hierarchical Visitor that visits all elements of a Handlebars-AST
  /// Hierarchical Visitors are described here: http://c2.com/cgi/wiki?HierarchicalVisitorPattern
  /// </summary>
  internal interface IASTVisitor
  {

    void VisitEnter(EachBlock astNode);
    void VisitLeave(EachBlock astNode);

    void VisitEnter(WithBlock astNode);
    void VisitLeave(WithBlock astNode);

    void VisitEnter(IfBlock astNode);
    void VisitLeave(IfBlock astNode);
    void VisitElse();

    void Visit(YieldStatement astLeaf);
    void Visit(MarkupLiteral astLeaf);
    void Visit(CommentLiteral astLeaf);

    void VisitEnter(HandlebarsTemplate template);
    void VisitLeave(HandlebarsTemplate template);
  }
}
