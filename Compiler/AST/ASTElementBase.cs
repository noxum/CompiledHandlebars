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

    internal ASTElementBase(int line, int column)
    {
      Line = line;
      Column = column;
    }

    internal abstract void Accept(IASTVisitor visitor);

    internal abstract bool HasElement<T>(bool includeChildren = false);
  }

  internal enum TokenType { Undefined, Encoded, Escaped }
}
