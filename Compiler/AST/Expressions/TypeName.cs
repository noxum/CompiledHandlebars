using CompiledHandlebars.Compiler.Introspection;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler.AST.Expressions
{

  internal class GenericTypeName : NamespaceOrTypeName
  {
    internal readonly IList<NamespaceOrTypeName> TypeParameters;

    internal GenericTypeName(string metadataName, IList<NamespaceOrTypeName> typeParameters) : base (metadataName)
    {
      TypeParameters = typeParameters;
    }

    internal override INamedTypeSymbol Evaluate(RoslynIntrospector introspector)
    {
      INamedTypeSymbol containingType = introspector.GetTypeSymbol($"{MetadataName}`{TypeParameters.Count}");
      return containingType.Construct(TypeParameters.Select(x => x.Evaluate(introspector)).ToArray());
    }

    public override string ToString()
    {
      return $"{MetadataName}<{string.Join(", ", TypeParameters.Select(x => x.ToString()))}>";
    }
  }

  internal class NamespaceOrTypeName
  {
    internal readonly string MetadataName;
    internal NamespaceOrTypeName(string metadataName)
    {
      MetadataName = metadataName;
    }

    internal virtual INamedTypeSymbol Evaluate(RoslynIntrospector introspector)
    {
      return introspector.GetTypeSymbol(MetadataName);
    }

    public override string ToString()
    {
      return MetadataName;
    }
  }
}
