using System.Text;
using System.Net;

/*10/16/2015 5:42:09 PM | parsing: 666ms; init: 1469; codeGeneration: 22!*/
namespace TestTemplates
{
  public static class UnlessTest
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      if (!IsTruthy(viewModel.Name))
      {
        sb.Append("HasNoName");
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