using System;
using System.Linq;
using System.Net;
using System.Text;

/*05.11.2015 10:55:36 | parsing: 0ms; init: 1; codeGeneration: 6!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class RootTest3
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Rovers))
      {
        foreach (var loopItem0 in viewModel.Rovers)
        {
          sb.Append(RootTest2.Render(viewModel));
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