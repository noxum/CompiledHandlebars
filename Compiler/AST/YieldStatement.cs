using System;
using CompiledHandlebars.Compiler.AST.Expressions;
using CompiledHandlebars.Compiler.Visitors;

namespace CompiledHandlebars.Compiler.AST
{
  internal class YieldStatement : ASTElementBase
  {
    internal readonly MemberExpression Member;
    internal TokenType Type { get; set; } = TokenType.Encoded;
    internal YieldStatement(MemberExpression member, int line, int column) : base(line, column)
    {
      Member = member;
    }

    internal void SetTokenType(TokenType type)
    {
      Type = type;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.Visit(this);
    }

    internal override bool HasElement<T>(bool includeChildren = false)
    {
      return (Member is T);
    }
  }
}
