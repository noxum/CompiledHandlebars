﻿using CompiledHandlebars.Compiler.Introspection;
using CompiledHandlebars.Compiler.Visitors;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CompiledHandlebars.Compiler
{
  public static class HbsCompiler
  {
    public static Tuple<string, IEnumerable<HandlebarsException>> Compile(string hbsTemplate, string solutionPath, string nameSpace, string name)
    {
      var parser = new HbsParser();
      try
      {
        var sw = new Stopwatch();
        sw.Start();
        var template = parser.Parse(hbsTemplate);
        sw.Stop();
        long parseTime = sw.ElapsedMilliseconds;
        template.Namespace = nameSpace;
        template.Name = name;
        sw.Restart();
        var codeGenerator = new CodeGenerationVisitor(new RoslynIntrospector(solutionPath), template);
        sw.Stop();
        long initTime = sw.ElapsedMilliseconds;
        sw.Restart();
        var state = codeGenerator.GenerateCode();
        sw.Stop();
        long generationTime = sw.ElapsedMilliseconds;
        return new Tuple<string, IEnumerable<HandlebarsException>>(state.GetCompilationUnit($"{DateTime.Now} | parsing: {parseTime}ms; init: {initTime}; codeGeneration: {generationTime}!").NormalizeWhitespace().ToFullString(), state.Errors);
      } catch(HandlebarsSyntaxError syntaxError)
      {
        return new Tuple<string, IEnumerable<HandlebarsException>>($"No result as SyntaxErrors occured: {syntaxError.Message}", new HandlebarsSyntaxError[] { syntaxError });
      }

    }
  }
}
