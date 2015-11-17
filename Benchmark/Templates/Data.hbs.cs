using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.Benchmark.Templates
{
  [CompiledHandlebarsTemplate]
  public static class Data
  {
    public static string Render(CompiledHandlebars.Benchmark.ViewModels.ArrayEachModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Names))
      {
        int index1 = 0;
        foreach (var loopItem0 in viewModel.Names)
        {
          sb.Append(WebUtility.HtmlEncode(index1.ToString()));
          sb.Append(WebUtility.HtmlEncode(loopItem0.Name));
          index1++;
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

    private class CompiledHandlebarsLayoutAttribute : Attribute
    {
    }
  }
}/*compiled in 35ms*/