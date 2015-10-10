using Microsoft.CodeAnalysis;

namespace CompiledHandlebars.Compiler.Introspection
{
  internal class Context
  {
    internal readonly string FullPath;
    internal readonly ISymbol Symbol;
    public Context(string fullPath, ISymbol symbol)
    {
      FullPath = fullPath;
      Symbol = symbol;
    }
  }
}
