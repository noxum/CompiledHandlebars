using System.Text;
using System.Net;

/*10/14/2015 9:35:38 PM | parsing: 0ms; init: 80; codeGeneration: 1!*/
namespace TestTemplates
{
    public static class BasicTest
    {
        public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
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