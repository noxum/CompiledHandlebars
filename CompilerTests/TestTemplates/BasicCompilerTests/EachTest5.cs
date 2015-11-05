using System;
using System.Linq;
using System.Net;
using System.Text;

/*05.11.2015 10:55:39 | parsing: 0ms; init: 1; codeGeneration: 0!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class EachTest5
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.StarModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Planets))
      {
        foreach (var loopItem0 in viewModel.Planets)
        {
          sb.Append(WebUtility.HtmlEncode(loopItem0.Name));
          sb.Append(";");
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

    private class CompiledHandlebarsTemplateAttribute : Attribute
    {
    }
  }
}/**/