using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.Helper
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public class RegisterHandlebarsTemplateAttribute : Attribute
  {
    public readonly string _contents;
    public readonly string _name;

    public RegisterHandlebarsTemplateAttribute(string name, string contents)
    {
      _contents = contents;
      _name = name;
    }
    public RegisterHandlebarsTemplateAttribute(string name, string contents, string modelToken)
    {
      _contents = string.Concat(modelToken,contents);
      _name = name;
    }
  }
}
