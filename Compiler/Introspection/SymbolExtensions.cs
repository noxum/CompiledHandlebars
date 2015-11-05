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
    public static ITypeSymbol FindMember(this ISymbol symbol, string name)
    {
      if (symbol == null)
        return null;
      if (symbol.Kind == SymbolKind.NamedType)
        return (symbol as INamedTypeSymbol).FindMemberRec(name);
      if (symbol.Kind == SymbolKind.Property)
        return (symbol as IPropertySymbol).Type?.FindMemberRec(name);
      return null;
    }

    /// <summary>
    /// Is used to get Symbols inside arrays, lists, enumerables etc.
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    public static ITypeSymbol GetElementSymbol(this ISymbol symbol)
    {
      if (symbol.Kind == SymbolKind.Property)
        return (symbol as IPropertySymbol).Type.GetElementSymbol();
      if (symbol.Kind == SymbolKind.ArrayType)
        return (symbol as IArrayTypeSymbol).ElementType;

      if (symbol.Kind == SymbolKind.NamedType && (symbol as INamedTypeSymbol).IsGenericType)
      {
        return (symbol as INamedTypeSymbol).AllInterfaces.First(x => x.MetadataName.Equals("IEnumerable`1")).TypeArguments.First();
      }
      return null;
    }

    /// <summary>
    /// Is Type under the symbol a string
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool IsString(this ISymbol symbol)
    {
      if (symbol.Kind == SymbolKind.Property)
        return (symbol as IPropertySymbol).Type.IsString();
      if (symbol.Kind == SymbolKind.NamedType)
        return (symbol as INamedTypeSymbol).SpecialType.HasFlag(SpecialType.System_String);
      return false;
    }

    private static ITypeSymbol FindMemberRec(this ITypeSymbol symbol, string name)
    {
      var result = symbol.GetMembers(name).FirstOrDefault();
      if (result == null && symbol.BaseType != null)
        //Ask base type
        return symbol.BaseType.FindMemberRec(name);
      if (!(result is ITypeSymbol))
      {
        if (result is IPropertySymbol)
          return (result as IPropertySymbol).Type;
      } else
        return (result as ITypeSymbol);
      return null;
    }
  }
}
