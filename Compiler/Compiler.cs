using CompiledHandlebars.Compiler.Introspection;
using CompiledHandlebars.Compiler.Visitors;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler
{
  public static class HbsCompiler
  {
    public static Tuple<string, IEnumerable<HandlebarsException>> Compile(string hbsTemplate, string solutionPath, string nameSpace, string name)
    {
      var parser = new HbsParser();
      try
      {
        var template = parser.Parse(hbsTemplate);
        template.Namespace = nameSpace;
        template.Name = name;
        var codeGenerator = new CodeGenerationVisitor(new RoslynIntrospector(solutionPath), template);
        var state = codeGenerator.GenerateCode();
        return new Tuple<string, IEnumerable<HandlebarsException>>(state.GetCompilationUnit().NormalizeWhitespace().ToFullString(), state.Errors);
      } catch(HandlebarsSyntaxError syntaxError)
      {
        return new Tuple<string, IEnumerable<HandlebarsException>>($"No result as SyntaxErrors occured: {syntaxError.Message}", new HandlebarsSyntaxError[] { syntaxError });
      }

    }
  }
}
