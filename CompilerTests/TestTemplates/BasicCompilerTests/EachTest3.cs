using System.Text;
using System.Net;

/*10/16/2015 5:43:17 PM | parsing: 0ms; init: 0; codeGeneration: 2!*/
namespace TestTemplates
{
  public static class EachTest3
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      foreach (var loopItem0 in viewModel.Rovers)
      {
        sb.Append(WebUtility.HtmlEncode(loopItem0.Key));
      }

      return sb.ToString();
    }

    public static bool IsTruthy(bool b)
    {
      return b;
    }

    public static bool IsTruthy(string s)
    {
      return !string.IsNullOrEmpty(s);
    }

    public static bool IsTruthy(object o)
    {
      return o != null;
    }
  }
}