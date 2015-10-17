using System.Text;
using System.Net;

/*10/16/2015 5:42:43 PM | parsing: 580ms; init: 33216; codeGeneration: 1!*/
namespace TestTemplates
{
  public static class WithTest
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel.Phobos))
      {
        sb.Append("Name:");
        sb.Append(WebUtility.HtmlEncode(viewModel.Phobos.Name));
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