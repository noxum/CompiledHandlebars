using CompiledHandlebars.Compiler.AST.Expressions;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler.AST
{
  internal abstract class ASTNode : ASTElementBase
  {
    protected readonly IEnumerable<ASTElementBase> _children;

    internal ASTNode(IEnumerable<ASTElementBase> children, int line, int column) : base(line, column)
    {
      _children = children;
    }
  }
}
