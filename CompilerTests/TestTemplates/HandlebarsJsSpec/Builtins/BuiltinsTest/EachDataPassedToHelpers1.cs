using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins
{
  [CompiledHandlebarsTemplate]
  public static class EachDataPassedToHelpers1
  {
    public static string Render(CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.LetterListModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Letters))
      {
        foreach (var loopItem0 in viewModel.Letters)
        {
          sb.Append(WebUtility.HtmlEncode(loopItem0));
          sb.Append(BuiltinsTest.DetectDataInsideEach(loopItem0));
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

    private static bool IsTruthy(int i)
    {
      return i != 0;
    }

    private class CompiledHandlebarsTemplateAttribute : Attribute
    {
    }
  }
}/*Line: 1; Column 112: Could not find Member 'DetectDataInsideEach' in Type 'string'!*/