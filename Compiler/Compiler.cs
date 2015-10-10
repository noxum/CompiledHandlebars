using CompiledHandlebars.Compiler.Visitors;
using System;
using System.Collections.Generic;

namespace CompiledHandlebars.Compiler
{
  public static class HbsCompiler
  {
    public static Tuple<string, IEnumerable<HandlebarsException>> Compile(string hbsTemplate, string solutionPath)
    {
      var parser = new HbsParser();
      var template = parser.Parse(hbsTemplate);
      var semanticAnalyzer = new SemanticAnalysisVisitor();
      var codeGenerator = new CodeGenerationVisitor();
      return new Tuple<string, IEnumerable<HandlebarsException>>(codeGenerator.GenerateCode(template), semanticAnalyzer.SemanticAnalysis(template));

    }
  }
}
