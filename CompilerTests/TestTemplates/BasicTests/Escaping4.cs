using System;
using System.Linq;
using System.Net;
using System.Text;

/**/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class Escaping4
  {
    public static string Render(CompiledHandlebars.CompilerTests.HandlebarsJsSpec.FooModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append("content \\");
      sb.Append(WebUtility.HtmlEncode(viewModel.Foo));
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