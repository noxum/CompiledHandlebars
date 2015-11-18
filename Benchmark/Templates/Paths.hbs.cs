using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.Benchmark.Templates
{
  [CompiledHandlebarsTemplate]
  public static class Paths
  {
    public static string Render(CompiledHandlebars.Benchmark.ViewModels.PathsModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append(WebUtility.HtmlEncode(viewModel.Person.Name.Bar.Baz));
      sb.Append(WebUtility.HtmlEncode(viewModel.Person.Age.ToString()));
      ; /* Not existing values do not compile {{person.foo}}{{animal.age}} */
      sb.Append("\r\n");
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
}/*compiled in 14ms*/