using System;

namespace CompiledHandlebars.Compiler
{
  public static class HbsCompiler
  {
    public static string Compile(string hbsTemplate, string solutionPath)
    {
      var parser = new HbsParser();
      var template = parser.Parse(hbsTemplate);
      return "Hello World!";
    }
  }
}
