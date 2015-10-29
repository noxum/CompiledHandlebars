using System.Linq;
using System.Text;
using System.Net;
using System;

/*29.10.2015 17:37:30 | parsing: 0ms; init: 1; codeGeneration: 0!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class FirstTest4
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      bool first0 = true;
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Plains))
      {
        foreach (var loopItem0 in viewModel.Plains)
        {
          if (!IsTruthy(first0))
          {
          }
          else
          {
            sb.Append(WebUtility.HtmlEncode(loopItem0.Name));
          }

          first0 = false;
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
}