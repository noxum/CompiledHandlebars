using CompiledHandlebars.RuntimeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CompiledHandlebars.ViewEngine
{
  public class CompiledHandlebarsViewEngine : VirtualPathProviderViewEngine
  {
    private Dictionary<string, Type> _mappings { get; set; }
    public CompiledHandlebarsViewEngine(Assembly assembly)
    {
      foreach(var template in assembly.GetTypes().Where(x => x.GetCustomAttribute(typeof(CompiledHandlebarsTemplateAttribute), false)!=null))
      {
        _mappings.Add(GetVirtualPath(assembly, template), template);
      }
    }

    /// <summary>
    /// Determines the VirtualPath for a Template.
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="template"></param>
    /// <returns></returns>
    private string GetVirtualPath(Assembly assembly, Type template)
    {
      return string.Concat("~/", template.Namespace.Substring(assembly.GetName().Name.Length + 1), "/", template.Name.Replace('.', '/'), ".hbs");
    }

    protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
    {
      if (_mappings.ContainsKey(partialPath))
        return new CompiledHandlebarsView(_mappings[partialPath]);
      return null;
    }

    protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
    {
      if (_mappings.ContainsKey(viewPath))
        return new CompiledHandlebarsView(_mappings[viewPath]);
      return null;
    }
  }
}
