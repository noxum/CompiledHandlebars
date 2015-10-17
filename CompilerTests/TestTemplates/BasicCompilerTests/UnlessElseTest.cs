using System.Text;
using System.Net;

/*10/16/2015 5:42:07 PM | parsing: 532ms; init: 1787; codeGeneration: 25!*/
namespace TestTemplates
{
  public static class UnlessElseTest
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      if (!IsTruthy(viewModel.Name))
      {
        sb.Append("HasNoName");
      }
      else
      {
        sb.Append("HasName");
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