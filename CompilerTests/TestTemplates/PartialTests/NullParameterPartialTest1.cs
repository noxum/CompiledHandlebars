using System.Linq;
using System.Text;
using System.Net;
using System;

/*29.10.2015 17:37:28 | parsing: 0ms; init: 0; codeGeneration: 5!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class NullParameterPartialTest1
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
}