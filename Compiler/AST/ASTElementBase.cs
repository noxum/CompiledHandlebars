using CompiledHandlebars.Compiler.Visitors;


namespace CompiledHandlebars.Compiler.AST
{
  internal abstract class ASTElementBase
  {
    /// <summary>
    /// Original Position in the template.
    /// Usefull for error messages
    /// </summary>
    internal readonly int Line;
    internal readonly int Column;
    internal TokenType _type { get; private set; }

    internal ASTElementBase(int line, int column)
    {
      Line = line;
      Column = column;
    }

    internal void SetTokenType(TokenType type)
    {
      _type = type;
    }

    internal abstract void Accept(IASTVisitor visitor);
  }

  internal enum TokenType { Undefined, Encoded, Escaped }
}
