using CompiledHandlebars.Compiler.Visitors;

namespace CompiledHandlebars.Compiler.AST
{
  internal class MarkupLiteral : ASTElementBase
  {
    internal readonly string _Value;
    internal readonly bool _TrimLeft;
    internal readonly bool _TrimRight;

    internal MarkupLiteral(string value, int line, int column) : base(line, column)
    {
      _Value = value;
    }

    internal MarkupLiteral(string value, bool trimLeft, bool trimRight, int line, int column) : base(line, column)
    {
      _TrimLeft = trimLeft;
      _TrimRight = trimRight;
      var tmpVal = value;
      if (trimLeft)
        tmpVal = tmpVal.TrimStart(' ','\n','\r','\t');
      if (trimRight)
        tmpVal = tmpVal.TrimEnd(' ', '\n', '\r', '\t');
      _Value = tmpVal;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
