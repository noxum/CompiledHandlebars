using CompiledHandlebars.Compiler.Visitors;


namespace CompiledHandlebars.Compiler.AST
{
  internal abstract class ASTElementBase
  {
    /// <summary>
    /// Original Position in the template.
    /// Usefull for error messages
    /// </summary>
    protected readonly int _line;
    protected readonly int _column;
    protected TokenType _type { get; set; }

    internal ASTElementBase(int line, int column)
    {
      _line = line;
      _column = column;
    }

    internal void SetTokenType(TokenType type)
    {
      _type = type;
    }

    internal abstract void Accept(IASTVisitor visitor);
  }

  internal enum TokenType { Undefined, Encoded, Escaped }
}
