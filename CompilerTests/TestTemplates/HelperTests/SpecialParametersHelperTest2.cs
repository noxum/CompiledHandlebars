using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests
{
  [CompiledHandlebarsTemplate]
  public static class SpecialParametersHelperTest2
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Plains))
      {
        int index1 = 0;
        bool last1 = false;
        bool first1 = true;
        foreach (var loopItem0 in viewModel.Plains)
        {
          last1 = index1 == (viewModel.Plains.Count() - 1);
          sb.Append("first:");
          sb.Append(";last:");
          index1++;
          first1 = false;
        }
      }

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
}/*Line: 1; Column 92: Could not find Helper Method 'BoolToYesNo'
Line: 1; Column 120: Could not find Helper Method 'BoolToYesNo'*/