using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec
{
  [CompiledHandlebarsTemplate]
  public static class CompileWithUndefinedContext1
  {
    public static string Render(CompiledHandlebars.CompilerTests.HandlebarsJsSpec.FooModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append("Goodbye\n");
      sb.Append("\n");
      sb.Append("!");
      return sb.ToString();
    }

    private static bool IsTruthy(bool b)
    {
      return b;
    }

    private static bool IsTruthy(string s)
    {
      return !string.IsNullOrEmpty(s);
    }

    private static bool IsTruthy(object o)
    {
      return o != null;
    }

    private static bool IsTruthy<T>(IEnumerable<T> ie)
    {
      return ie != null && ie.Any();
    }

    private class CompiledHandlebarsTemplateAttribute : Attribute
    {
    }
  }
}/*Line: 2; Column 3: Could not find Member 'cruel' in Type 'CompiledHandlebars.CompilerTests.HandlebarsJsSpec.FooModel'!
Line: 2; Column 3: Could not find Helper Method 'cruel'
Line: 3; Column 3: Could not find Member 'world' in Type 'CompiledHandlebars.CompilerTests.HandlebarsJsSpec.FooModel'!
Line: 3; Column 3: Could not find Helper Method 'world.bar'*/