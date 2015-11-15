using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace CompiledHandlebars.Compiler.Introspection
{

  internal class CompileTimeLoopContext : Context
  {
    internal readonly List<Context> Members;
    internal int CurrentMemberIndex { get; set; }

    internal CompileTimeLoopContext(string fullPath, ITypeSymbol symbol) :base(fullPath, symbol)
    {
      //Get all Members of symbol which are accessible properties and build a context for each of them
      Members = symbol.GetMembers().OfType<IPropertySymbol>()
                                   .Where(x => x.DeclaredAccessibility == Accessibility.Public || x.DeclaredAccessibility == Accessibility.Internal)
                                   .Select(x => new Context($"{fullPath}.{x.Name}", x.Type)).ToList();
    }
  }
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
