using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests
{
  [CompiledHandlebarsTemplate]
  public static class IDerivedIntegerTest
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.IDerived viewModel)
    {
      var sb = new StringBuilder(64);
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

    private static bool IsTruthy(int i)
    {
      return i != 0;
    }

    private class CompiledHandlebarsTemplateAttribute : Attribute
    {
    }

    private class CompiledHandlebarsLayoutAttribute : Attribute
    {
    }
  }
}/*Line: 1; Column 69: Could not find Member 'AnotherInt' in Type 'CompiledHandlebars.CompilerTests.TestViewModels.IDerived'!
Line: 1; Column 69: Could not find Helper Method 'AnotherInt'*/