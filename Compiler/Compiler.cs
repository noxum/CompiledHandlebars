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
    public static Tuple<string, IEnumerable<HandlebarsException>> Compile(string hbsTemplate, string nameSpace, string name, Project project, bool includeTimestamp = true)
    {
      var parser = new HbsParser();
      try
      {
        var sw = new Stopwatch();
        sw.Start();                
        var template = parser.Parse(hbsTemplate);
        long parseTime = sw.ElapsedMilliseconds;        
        template.Namespace = nameSpace;
        template.Name = name;
        sw.Restart();
        if(!(template._ParseErrors?.Any()?? false))
        {//No parser errors
          var codeGenerator = new CodeGenerationVisitor(new RoslynIntrospector(project), template);
          if (!codeGenerator.ErrorList.Any())
          {//No code generator initialization errors
            long initTime = sw.ElapsedMilliseconds;
            sw.Restart();
            codeGenerator.GenerateCode();
            sw.Stop();
            long generationTime = sw.ElapsedMilliseconds;
            return new Tuple<string, IEnumerable<HandlebarsException>>(
              codeGenerator.CompilationUnit(
                includeTimestamp ? $"{DateTime.Now} | parsing: {parseTime}ms; init: {initTime}; codeGeneration: {generationTime}!"
                                 : string.Empty
              ).NormalizeWhitespace(indentation: "  ").ToFullString(), codeGenerator.ErrorList);
          }
          return new Tuple<string, IEnumerable<HandlebarsException>>(string.Empty, codeGenerator.ErrorList);
        }
        return new Tuple<string, IEnumerable<HandlebarsException>>(string.Empty, template._ParseErrors);
      } catch(HandlebarsSyntaxError syntaxError)
      {
        return new Tuple<string, IEnumerable<HandlebarsException>>($"No result as SyntaxErrors occured: {syntaxError.Message}", new HandlebarsSyntaxError[] { syntaxError });
      }
    }
  }
}
