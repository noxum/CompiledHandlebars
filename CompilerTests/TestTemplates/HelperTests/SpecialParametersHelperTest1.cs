using System;
using System.Linq;
using System.Net;
using System.Text;
using CompiledHandlebars.CompilerTests;

/*05.11.2015 10:55:34 | parsing: 4ms; init: 1; codeGeneration: 13!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class SpecialParametersHelperTest1
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Plains))
      {
        int index1 = 0;
        foreach (var loopItem0 in viewModel.Plains)
        {
          sb.Append(HelperTests.IndexPlusOne(index1));
          sb.Append(":");
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

    private class CompiledHandlebarsTemplateAttribute : Attribute
    {
    }
  }
}/**/