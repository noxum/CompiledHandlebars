using CompiledHandlebars.Compiler.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler.Visitors
{
  /// <summary>
  /// This is the interface for a hierarchical Visitor that visits all elements of a Handlebars-AST
  /// </summary>
  interface IASTVisitor
  {

    void Visit(YieldStatement astLeaf);
    void Visit(MarkupLiteral astLeaf);

    void VisitEnter(HandlebarsTemplate template);
    void VisitLeave(HandlebarsTemplate template);
  }
}
