using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

/**/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class EachTest4
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.StarModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Planets))
      {
        foreach (var loopItem0 in viewModel.Planets)
        {
          sb.Append(WebUtility.HtmlEncode(loopItem0.Name));
          sb.Append(":");
          if (IsTruthy(loopItem0) && IsTruthy(loopItem0.Moons))
          {
            foreach (var loopItem1 in loopItem0.Moons)
            {
              sb.Append(WebUtility.HtmlEncode(loopItem1.Name));
            }
          }

          sb.Append(";");
        }
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

    private static bool IsTruthy<T>(IEnumerable<T> ie)
    {
      return ie != null && ie.Any();
    }

    private class CompiledHandlebarsTemplateAttribute : Attribute
    {
    }
  }
}/**/