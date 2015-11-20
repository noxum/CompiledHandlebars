using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.Benchmark.ViewModels.MeasurementModels.Templates.Templates
{
  [CompiledHandlebarsTemplate]
  public static class Partial
  {
    public static string Render(CompiledHandlebars.Benchmark.ViewModels.PartialModel viewModel)
    {
      var sb = new StringBuilder(64);
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Peeps))
      {
        foreach (var loopItem0 in viewModel.Peeps)
        {
          sb.Append(Variables.Render(loopItem0));
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

    private static bool IsTruthy(int i)
    {
      return i != 0;
    }

    private class CompiledHandlebarsTemplateAttribute : Attribute
    {
    }

    private class CompiledHandlebarsLayoutAttribute : Attribute
    {
    }
  }
}