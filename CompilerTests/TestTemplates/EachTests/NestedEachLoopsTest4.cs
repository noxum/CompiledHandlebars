using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests
{
  [CompiledHandlebarsTemplate]
  public static class NestedEachLoopsTest4
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.PageListModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Items))
      {
        bool first1 = true;
        foreach (var loopItem0 in viewModel.Items)
        {
          if (IsTruthy(first1))
          {
            if (IsTruthy(loopItem0))
            {
              sb.Append(WebUtility.HtmlEncode(loopItem0.Title));
              sb.Append(WebUtility.HtmlEncode(loopItem0.Headline));
            }
          }

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

    private static bool IsTruthy(int i)
    {
      return i != 0;
    }

    private class CompiledHandlebarsTemplateAttribute : Attribute
    {
    }
  }
}/**/