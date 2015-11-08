using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace TestTemplates
{
  [CompiledHandlebarsLayout]
  public static class LayoutsWithInterfaceModelTest1
  {
    public static string PostRender(CompiledHandlebars.CompilerTests.TestViewModels.IPageModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append("</body>");
      return sb.ToString();
    }

    public static string PreRender(CompiledHandlebars.CompilerTests.TestViewModels.IPageModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append("<head><title>");
      sb.Append(WebUtility.HtmlEncode(viewModel.Title));
      sb.Append("</title></head><body>");
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

    private class CompiledHandlebarsLayoutAttribute : Attribute
    {
    }
  }
}/**/