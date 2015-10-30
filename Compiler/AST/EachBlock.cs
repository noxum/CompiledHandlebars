using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.Visitors;
using CompiledHandlebars.Compiler.AST.Expressions;

namespace CompiledHandlebars.Compiler.AST
{
  internal class EachBlock : ASTNode
  {
    internal enum ForLoopFlags { None = 0, First = 1, Last = 2, Index = 4 }

    internal readonly MemberExpression Member;
    private ForLoopFlags? _flags = null;
    internal ForLoopFlags Flags
    {
      get
      {
        if (!_flags.HasValue)
        {
          _flags = ForLoopFlags.None;
          if (_children.Any(x => x.HasExpressionOnLoopLevel<FirstExpression>()))
            _flags |= ForLoopFlags.First;
          if (_children.Any(x => x.HasExpressionOnLoopLevel<LastExpression>()))
            _flags |= ForLoopFlags.Last;
          if (_children.Any(x => x.HasExpressionOnLoopLevel<IndexExpression>()))
            _flags |= ForLoopFlags.Index;
        }
        return _flags.Value;
      }
    }
    internal EachBlock(MemberExpression member, IList<ASTElementBase> children, int line, int column) : base(children, line, column)
    {
      Member = member;
    }
    internal override void Accept(IASTVisitor visitor)
    {
      visitor.VisitEnter(this);
      foreach (var child in _children)
        child.Accept(visitor);
      visitor.VisitLeave(this);
    }

    internal override bool HasExpressionOnLoopLevel<T>()
    {
      return (Member is T);
    }
  }
}
