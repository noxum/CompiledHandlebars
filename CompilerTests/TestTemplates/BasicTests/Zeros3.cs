using System;
using System.Linq;
using System.Net;
using System.Text;

/**/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class Zeros3
  {
    public static string Render(CompiledHandlebars.CompilerTests.HandlebarsJsSpec.NumModel2 viewModel)
    {
      var sb = new StringBuilder();
      sb.Append("num: ");
      sb.Append(WebUtility.HtmlEncode(viewModel.Num1.Num2.ToString()));
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

    private class CompiledHandlebarsTemplateAttribute : Attribute
    {
    }
  }
}/**/