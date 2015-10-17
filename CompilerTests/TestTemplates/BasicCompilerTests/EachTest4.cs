using System.Text;
using System.Net;

/*10/16/2015 5:43:17 PM | parsing: 1ms; init: 0; codeGeneration: 0!*/
namespace TestTemplates
{
  public static class EachTest4
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.StarModel viewModel)
    {
      var sb = new StringBuilder();
      foreach (var loopItem0 in viewModel.Planets)
      {
        sb.Append(WebUtility.HtmlEncode(loopItem0.Name));
        sb.Append(":");
        foreach (var loopItem1 in loopItem0.Moons)
        {
          sb.Append(WebUtility.HtmlEncode(loopItem1.Name));
        }

        sb.Append(";");
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