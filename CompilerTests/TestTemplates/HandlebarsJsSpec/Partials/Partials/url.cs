using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials
{
  [CompiledHandlebarsTemplate]
  public static class url
  {
    public static string Render(CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Partials.DudeModel viewModel)
    {
      var sb = new StringBuilder(64);
      sb.Append("<a href=\"");
      sb.Append(WebUtility.HtmlEncode(viewModel.Url));
      sb.Append("\">");
      sb.Append(WebUtility.HtmlEncode(viewModel.Url));
      sb.Append("</a>");
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
}/**/