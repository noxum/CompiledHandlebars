using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using CompiledHandlebars.CompilerTests;

namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class ImplicitThisParameterTest1
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append(HelperTests.MarsHelper(viewModel));
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
}/*Line: 1; Column 70: Could not find Member 'MarsHelper' in Type 'CompiledHandlebars.CompilerTests.TestViewModels.MarsModel'!*/