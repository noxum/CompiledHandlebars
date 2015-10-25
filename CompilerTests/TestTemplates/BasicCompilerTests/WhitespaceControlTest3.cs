using System.Text;
using System.Net;
using System;

/*10/24/2015 5:49:28 PM | parsing: 4ms; init: 8; codeGeneration: 2!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class WhitespaceControlTest3
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append(viewModel.Name);
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