﻿using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins
{
  [CompiledHandlebarsTemplate]
  public static class EachWithNestedFirst1
  {
    public static string Render(CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.TextListModel viewModel)
    {
      var sb = new StringBuilder();
      if (IsTruthy(viewModel) && IsTruthy(viewModel.Goodbyes))
      {
        bool first1 = true;
        foreach (var loopItem0 in viewModel.Goodbyes)
        {
          sb.Append("(");
          if (IsTruthy(first1))
          {
            sb.Append(WebUtility.HtmlEncode(loopItem0.Text));
            sb.Append("! ");
          }

          bool first2 = true;
          foreach (var loopItem1 in viewModel.Goodbyes)
          {
            if (IsTruthy(first2))
            {
              sb.Append(WebUtility.HtmlEncode(loopItem1.Text));
              sb.Append("!");
            }

            first2 = false;
          }

          if (IsTruthy(first1))
          {
            sb.Append(" ");
            sb.Append(WebUtility.HtmlEncode(loopItem0.Text));
            sb.Append("!");
          }

          sb.Append(") ");
          first1 = false;
        }
      }

      sb.Append("cruel ");
      sb.Append(WebUtility.HtmlEncode(viewModel.World));
      sb.Append("!");
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
  }
}/**/