using System.Text;
using System.Net;

/*13.10.2015 18:19:00 | parsing: 0ms; init: 0; codeGeneration: 0!*/
namespace TestTemplates
{
    public static class MarsConditionals
    {
        public static string Render(CompiledHandlebars.Compiler.Tests.MarsModel viewModel)
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