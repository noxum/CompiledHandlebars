using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace CompiledHandlebars.Compiler.Introspection
{

  internal class CompileTimeLoopContext : Context
  {
    internal readonly List<Context> Members;
    internal int Index { get; set; }
    internal bool First { get; set; }
    internal bool Last { get; set; }
    internal string Key { get; set; }
    internal CompileTimeLoopContext(string fullPath, ITypeSymbol symbol) :base(fullPath, symbol)
    {
      //Get all Members of symbol which are accessible properties and build a context for each of them
      Members = symbol.GetMembers().OfType<IPropertySymbol>()
                                   .Where(x => x.DeclaredAccessibility == Accessibility.Public || x.DeclaredAccessibility == Accessibility.Internal)
                                   .Select(x => new Context($"{fullPath}.{x.Name}", x.Type)).ToList();
      Index = -1; //To make MoveNext behave like other MoveNexts (e.g. First call MoveNext before you do anything else);
    }

    internal bool MoveNext()
    {
      Index++;
      First = Index == 0;
      Last = Index == Members.Count - 1;
      if (Index < Members.Count)
      {
        Key = Members[Index].FullPath.Split('.').Last();
        FullPath = Members[Index].FullPath;
        Symbol = Members[Index].Symbol;
        return true;
      }
      else
        return false;
    }
  }
  internal class Context
  {
    internal string FullPath { get; set; }
    internal ITypeSymbol Symbol { get; set; }

    internal bool Truthy { get; set; }
    public Context(string fullPath, ITypeSymbol symbol)
    {
      FullPath = fullPath;
      Symbol = symbol;
    }
  }
}
