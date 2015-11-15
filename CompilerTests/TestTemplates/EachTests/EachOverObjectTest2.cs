using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests
{
  [CompiledHandlebarsTemplate]
  public static class EachOverObjectTest2
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.PageModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel))
      {
        sb.Append(WebUtility.HtmlEncode(0.ToString()));
        sb.Append(WebUtility.HtmlEncode(viewModel.Title));
        sb.Append(WebUtility.HtmlEncode(1.ToString()));
        sb.Append(WebUtility.HtmlEncode(viewModel.Headline));
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