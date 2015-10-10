using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
