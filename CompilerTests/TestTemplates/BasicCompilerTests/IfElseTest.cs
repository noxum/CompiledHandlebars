using System.Text;
using System.Net;

/*10/16/2015 5:42:00 PM | parsing: 582ms; init: 5965; codeGeneration: 40!*/
namespace TestTemplates
{
  public static class IfElseTest
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel.Name))
      {
        sb.Append("HasName");
      }
      else
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