using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests
{
  [CompiledHandlebarsTemplate]
  public static class BasicLayoutTest2
  {
    public static string Render(System.String viewModel)
    {
      var sb = new StringBuilder();
      sb.Append(BasicLayoutTest1.PreRender(viewModel));
      sb.Append(WebUtility.HtmlEncode(viewModel));
      sb.Append(BasicLayoutTest1.PostRender(viewModel));
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
}/**/