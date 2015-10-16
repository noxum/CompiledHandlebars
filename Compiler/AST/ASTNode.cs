using CompiledHandlebars.Compiler.AST.Expressions;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler.AST
{
  internal abstract class ASTNode : ASTElementBase
  {
    protected readonly IEnumerable<ASTElementBase> _children;
    internal readonly MemberExpression Member;

    internal ASTNode(MemberExpression member, IEnumerable<ASTElementBase> children, int line, int column) : base(line, column)
    {
      Member = member;
      _children = children;
    }
  }
}
