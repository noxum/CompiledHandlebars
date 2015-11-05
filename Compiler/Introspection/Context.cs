using Microsoft.CodeAnalysis;

namespace CompiledHandlebars.Compiler.Introspection
{
  internal class Context
  {
    internal readonly string FullPath;
    internal readonly ITypeSymbol Symbol;

    internal bool Truthy { get; set; }
    public Context(string fullPath, ITypeSymbol symbol)
    {
      FullPath = fullPath;
      Symbol = symbol;
    }
  }
}
