using System;
using System.Linq;
using System.Net;
using System.Text;

/**/
namespace TestTemplates
{
  [CompiledHandlebarsTemplate]
  public static class CompileWithUndefinedContext2
  {
    public static string Render(CompiledHandlebars.CompilerTests.HandlebarsJsSpec.FooModel viewModel)
    {
      var sb = new StringBuilder();
      if (!IsTruthy(viewModel) || !IsTruthy(viewModel.Foo))
      {
        sb.Append("Goodbye");
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
}/*Line: 1; Column 93: Error in MemberExpression: Empty ContextStack but PathUp Element ('../')!
Line: 1; Column 93: Could not find Helper Method '../test'
Line: 1; Column 104: Could not find Member 'test2' in Type 'CompiledHandlebars.CompilerTests.HandlebarsJsSpec.FooModel'!
Line: 1; Column 104: Could not find Helper Method 'test2'*/