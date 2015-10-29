using CompiledHandlebars.Compiler.AST.Expressions;
using CompiledHandlebars.Compiler.Visitors;

namespace CompiledHandlebars.Compiler.AST
{
  internal class YieldStatement : ASTElementBase
  {
    internal readonly Expression Member;

    internal YieldStatement(Expression member, int line, int column) : base(line, column)
    {
      Member = member;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
