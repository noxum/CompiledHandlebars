using System.Text;
using System.Net;

/*10/16/2015 1:15:04 PM | parsing: 0ms; init: 0; codeGeneration: 0!*/
namespace TestTemplates
{
    public static class NestedIfTest
    {
        public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
        {
            var sb = new StringBuilder();
            if (IsTruthy(viewModel.Phobos))
            {
                sb.Append("Phobos:");
                if (IsTruthy(viewModel.Phobos.Name))
                {
                    sb.Append("HasName");
                }
                else
                {
                    sb.Append("HasNoName");
                }
            }
            else
            {
                sb.Append("NoPhobos");
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