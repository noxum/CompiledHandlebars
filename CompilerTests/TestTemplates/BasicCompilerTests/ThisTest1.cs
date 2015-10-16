using System.Text;
using System.Net;

/*10/16/2015 1:15:04 PM | parsing: 1ms; init: 2; codeGeneration: 0!*/
namespace TestTemplates
{
    public static class ThisTest1
    {
        public static string Render(System.String viewModel)
        {
            var sb = new StringBuilder();
            sb.Append(WebUtility.HtmlEncode(viewModel));
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