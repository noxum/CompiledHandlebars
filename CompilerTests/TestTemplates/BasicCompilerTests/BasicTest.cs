using System.Text;
using System.Net;

/*10/16/2015 5:40:56 PM | parsing: 79ms; init: 720; codeGeneration: 69!*/
namespace TestTemplates
{
  public static class BasicTest
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append(WebUtility.HtmlEncode(viewModel.Name));
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