using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNetFramework
{
    [CompiledHandlebarsTemplate]
    public static class Template
    {
        public static async Task RenderAsync(DotNetFramework.ViewModel viewModel, StringBuilder sb)
        {
            sb.Append("\r\nHello ");
            sb.Append(WebUtility.HtmlEncode(viewModel.Name));
            sb.Append("!");
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