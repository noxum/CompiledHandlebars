using System;
using System.Linq;
using System.Net;
using System.Text;

/*05.11.2015 10:55:39 | parsing: 0ms; init: 108; codeGeneration: 2!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class BasicTest
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append(WebUtility.HtmlEncode(viewModel.Name));
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