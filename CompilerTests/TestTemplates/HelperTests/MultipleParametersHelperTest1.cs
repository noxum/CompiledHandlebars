using System;
using System.Linq;
using System.Net;
using System.Text;
using CompiledHandlebars.CompilerTests;

/*05.11.2015 10:55:34 | parsing: 0ms; init: 1; codeGeneration: 27!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class MultipleParametersHelperTest1
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append(HelperTests.IsMoonOf(viewModel, viewModel.Deimos));
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
}/**/