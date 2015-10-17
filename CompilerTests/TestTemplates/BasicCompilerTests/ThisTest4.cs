﻿using System.Text;
using System.Net;

/*10/16/2015 5:43:17 PM | parsing: 0ms; init: 0; codeGeneration: 0!*/
namespace TestTemplates
{
  public static class ThisTest4
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel))
      {
        sb.Append("Model");
      }
      else
      {
        sb.Append("NoModel");
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