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
    public static ISymbol FindMember(this ISymbol symbol, string name)
    {
      if (symbol.Kind == SymbolKind.NamedType)
        return (symbol as INamedTypeSymbol).FindMemberRec(name);
      if (symbol.Kind == SymbolKind.Property)
        return (symbol as IPropertySymbol).Type?.FindMemberRec(name);
      return null;
    }

    private static ISymbol FindMemberRec(this ITypeSymbol symbol, string name)
    {
      var result = symbol.GetMembers(name).FirstOrDefault();
      if (result == null && symbol.BaseType != null)
        //Ask base type
        return symbol.BaseType.FindMemberRec(name);
      return result;
    }
  }
}
