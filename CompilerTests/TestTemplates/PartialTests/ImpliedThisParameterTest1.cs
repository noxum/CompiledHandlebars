using System;
using System.Linq;
using System.Net;
using System.Text;

/*05.11.2015 10:55:35 | parsing: 0ms; init: 0; codeGeneration: 5!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class ImpliedThisParameterTest1
  {
    public static string Render(System.String viewModel)
    {
      var sb = new StringBuilder();
      sb.Append(BasicPartialTest1.Render(viewModel));
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