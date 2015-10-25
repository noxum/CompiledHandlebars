using System.Text;
using System.Net;
using System;

/*10/24/2015 9:10:34 PM | parsing: 3ms; init: 1; codeGeneration: 7!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class CommentTest1
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      ; /*Name*/
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