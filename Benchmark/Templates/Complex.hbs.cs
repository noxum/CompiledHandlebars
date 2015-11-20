using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.Benchmark.ViewModels.MeasurementModels.Templates.Templates
{
  [CompiledHandlebarsTemplate]
  public static class Complex
  {
    public static string Render(CompiledHandlebars.Benchmark.ViewModels.ComplexModel viewModel)
    {
      var sb = new StringBuilder(64);
      sb.Append("<h1>");
      sb.Append(WebUtility.HtmlEncode(viewModel.Header));
      sb.Append("</h1>\r\n");
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Items))
      {
        sb.Append("\r\n  <ul>\r\n    ");
        foreach (var loopItem0 in viewModel.Items)
        {
          sb.Append("\r\n      ");
          if (IsTruthy(loopItem0) && IsTruthy(loopItem0.Current))
          {
            sb.Append("\r\n        <li><strong>");
            sb.Append(WebUtility.HtmlEncode(loopItem0.Name));
            sb.Append("</strong></li>\r\n      ");
          }
          else
          {
            sb.Append("\r\n        <li><a href=\"");
            sb.Append(WebUtility.HtmlEncode(loopItem0.Url));
            sb.Append("\">");
            sb.Append(WebUtility.HtmlEncode(loopItem0.Name));
            sb.Append("</a></li>\r\n      ");
          }

          sb.Append("\r\n    ");
        }

        sb.Append("\r\n  </ul>\r\n");
      }
      else
      {
        sb.Append("\r\n  <p>The list is empty.</p>\r\n");
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

    private class CompiledHandlebarsLayoutAttribute : Attribute
    {
    }
  }
}