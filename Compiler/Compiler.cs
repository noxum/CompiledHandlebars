using CompiledHandlebars.Compiler.Introspection;
using CompiledHandlebars.Compiler.Visitors;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CompiledHandlebars.Compiler
{
  public static class HbsCompiler
  {
    public static Tuple<string, IEnumerable<HandlebarsException>> Compile(string content, string @namespace, string name, Project containingProject)
    {
      var parser = new HbsParser();         
      var template = parser.Parse(content);
      template.Namespace = @namespace;
      template.Name = name;
      if(!(template.ParseErrors?.Any()?? false))
      {//No parser errors
        var codeGenerator = new CodeGenerationVisitor(new RoslynIntrospector(containingProject), template);
        if (!codeGenerator.ErrorList.Any())
        {//No code generator initialization errors
          return new Tuple<string, IEnumerable<HandlebarsException>>(
            codeGenerator.GenerateCode().NormalizeWhitespace(indentation: "  ").ToFullString(), codeGenerator.ErrorList);
        }          
        return new Tuple<string, IEnumerable<HandlebarsException>>(string.Empty, codeGenerator.ErrorList);
      }
      return new Tuple<string, IEnumerable<HandlebarsException>>(string.Empty, template.ParseErrors);      
    }
  }
}
