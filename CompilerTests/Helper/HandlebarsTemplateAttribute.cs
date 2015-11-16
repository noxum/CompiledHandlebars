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
    public readonly string _overridenNameSpace;
    public readonly bool _include;

    public RegisterHandlebarsTemplateAttribute(string name, string contents, bool includeInAssembly = true)
    {
      _contents = contents;
      _name = name;
      _include = includeInAssembly;
    }
    public RegisterHandlebarsTemplateAttribute(string name, string contents, string modelToken, bool includeInAssembly = true)
    {
      _contents = string.Concat(modelToken,contents);
      _name = name;
      _include = includeInAssembly;
    }

    public RegisterHandlebarsTemplateAttribute(string name, string contents, string modelToken, string @namespace)
    {
      _contents = string.Concat(modelToken, contents);
      _name = name;
      _include = true;
      _overridenNameSpace = @namespace;
    }
  }
}
