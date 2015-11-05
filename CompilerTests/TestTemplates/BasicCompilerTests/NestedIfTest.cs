using System;
using System.Linq;
using System.Net;
using System.Text;

/*11/4/2015 10:20:04 PM | parsing: 0ms; init: 3; codeGeneration: 0!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class NestedIfTest
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Phobos))
      {
        sb.Append("Phobos:");
        if (IsTruthy(viewModel.Phobos.Name))
        {
          sb.Append("HasName");
        }
        else
        {
          sb.Append("HasNoName");
        }
      }
      else
      {
        sb.Append("NoPhobos");
      }

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
}/*Line: 1; Column 123: Unreachable code detected*/