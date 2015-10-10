using CompiledHandlebars.Compiler.AST;

namespace CompiledHandlebars.Compiler.Visitors
{
  /// <summary>
  /// This is the interface for a hierarchical Visitor that visits all elements of a Handlebars-AST
  /// Hierarchical Visitors are described here: http://c2.com/cgi/wiki?HierarchicalVisitorPattern
  /// </summary>
  interface IASTVisitor
  {

    void Visit(YieldStatement astLeaf);
    void Visit(MarkupLiteral astLeaf);

    void VisitEnter(HandlebarsTemplate template);
    void VisitLeave(HandlebarsTemplate template);
  }
}
