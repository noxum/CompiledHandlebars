using System.Text;
using System.Net;

/*13.10.2015 18:19:00 | parsing: 0ms; init: 48; codeGeneration: 1!*/
namespace TestTemplates
{
    public static class Mars
    {
        public static string Render(CompiledHandlebars.Compiler.Tests.MarsModel viewModel)
        {
            var sb = new StringBuilder();
            sb.Append(WebUtility.HtmlEncode(viewModel.Name));
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