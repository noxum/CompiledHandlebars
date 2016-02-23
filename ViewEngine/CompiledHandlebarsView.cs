using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CompiledHandlebars.ViewEngine
{
  public class CompiledHandlebarsView : IView
  {
    private readonly Type _templateType;    

    public CompiledHandlebarsView(Type templateType)
    {
      _templateType = templateType;
    }

    public void Render(ViewContext viewContext, TextWriter writer)
    {
      MethodInfo renderMethod = _templateType.GetMethod("Render");
      string output;
      if (!viewContext.HttpContext.Items.Contains("CompiledHanldbearsHtmlHelper"))
      {        
        var helper = new HtmlHelper(viewContext, new MockViewDataContainer(viewContext.ViewData));
        viewContext.HttpContext.Items["CompiledHandlebarsHtmlHelper"] = helper;      
      }
      if (renderMethod.GetParameters().Any())
      {
        output = (string)renderMethod.Invoke(null, new object[1] { viewContext.ViewData?.Model });
      } else
      {
        output = (string)renderMethod.Invoke(null, null);
      }
      writer.Write(output);
    }

    public class MockViewDataContainer : IViewDataContainer
    {
      public MockViewDataContainer(ViewDataDictionary viewData)
      {
        ViewData = viewData;
      }
      public ViewDataDictionary ViewData { get; set; }      
    }
  }
}
