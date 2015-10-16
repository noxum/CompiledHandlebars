﻿using System.Text;
using System.Net;

/*10/16/2015 1:15:04 PM | parsing: 1ms; init: 0; codeGeneration: 1!*/
namespace TestTemplates
{
    public static class IfElseTest
    {
        public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
        {
            var sb = new StringBuilder();
            if (IsTruthy(viewModel.Name))
            {
                sb.Append("HasName");
            }
            else
            {
                sb.Append("HasNoName");
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