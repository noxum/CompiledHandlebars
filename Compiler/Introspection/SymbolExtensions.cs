using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler.Introspection
{
  public static class SymbolExtensions
  {
    public static ISymbol FindMemberIfExists(this ISymbol symbol, string name)
    {
      if (symbol.Kind == SymbolKind.NamedType)
        return (symbol as INamedTypeSymbol).GetMembers(name).FirstOrDefault();
      if (symbol.Kind == SymbolKind.Property)
        return (symbol as IPropertySymbol).Type?.GetMembers(name).FirstOrDefault();
      return null;
    }
  }
}
