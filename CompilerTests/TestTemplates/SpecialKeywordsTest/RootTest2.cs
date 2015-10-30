﻿using System.Linq;
using System.Text;
using System.Net;
using System;

/*29.10.2015 17:37:30 | parsing: 0ms; init: 1; codeGeneration: 17!*/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class RootTest2
  {
    public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Rovers))
      {
        foreach (var loopItem0 in viewModel.Rovers)
        {
          sb.Append(RootTest3.Render(viewModel));
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

    private class CompiledHandlebarsTemplateAttribute : Attribute
    {
    }
  }
}