using System.Linq;
using System.Text;
using System.Net;
using System;
using CompiledHandlebars.CompilerTests;

/*04.11.2015 15:35:14 | parsing: 1ms; init: 79; codeGeneration: 19!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class BasicHelperTest1
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      sb.Append(HelperTests.ToUpper(viewModel.Name));
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