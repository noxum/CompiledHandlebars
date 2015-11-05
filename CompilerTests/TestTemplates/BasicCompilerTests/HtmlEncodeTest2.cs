using System.Linq;
using System.Text;
using System.Net;
using System;

/*11/4/2015 4:50:22 PM | parsing: 2ms; init: 3; codeGeneration: 1!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class HtmlEncodeTest2
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append(viewModel.Description);
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