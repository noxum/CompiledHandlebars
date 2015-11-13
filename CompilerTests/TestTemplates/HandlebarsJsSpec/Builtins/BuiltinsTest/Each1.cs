using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins
{
  [CompiledHandlebarsTemplate]
  public static class Each1
  {
    public static string Render(CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.TextListModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Goodbyes))
      {
        foreach (var loopItem0 in viewModel.Goodbyes)
        {
          sb.Append(WebUtility.HtmlEncode(loopItem0.Text));
          sb.Append("! ");
        }
      }

      sb.Append("cruel ");
      sb.Append(WebUtility.HtmlEncode(viewModel.World));
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

    private static bool IsTruthy(int i)
    {
      return i != 0;
    }

    private class CompiledHandlebarsTemplateAttribute : Attribute
    {
    }
  }
}/**/