using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.RuntimeUtils
{
  public static class RenderHelper
  {
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

    public static bool IsTruthy<T>(IEnumerable<T> ie)
    {
      return (ie != null && ie.Any());
    }

  }

  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class CompiledHandlebarsTemplateAttribute : Attribute { }

  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class CompiledHandlebarsLayoutAttribute : Attribute
  {
  }

  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
  public class CompiledHandlebarsHelperMethodAttribute : Attribute { }
}
