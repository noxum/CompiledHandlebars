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
    private readonly RenderMethodWrapperBase _funcWrapper;    

    public CompiledHandlebarsView(RenderMethodWrapperBase funcWrapper)
    {
      _funcWrapper = funcWrapper;
    }

    public void Render(ViewContext viewContext, TextWriter writer)
    {
      //If there is no HtmlHelper in the HttpContext.Items yet, provide it
      if (!viewContext.HttpContext.Items.Contains("CompiledHanldbearsHtmlHelper"))
      {        
        var helper = new HtmlHelper(viewContext, new ViewDataContainer(viewContext.ViewData));
        viewContext.HttpContext.Items["CompiledHandlebarsHtmlHelper"] = helper;      
      }
      string output = _funcWrapper.InvokeRender(viewContext.ViewData?.Model);
      writer.Write(output);
    }

    public class ViewDataContainer : IViewDataContainer
    {
      public ViewDataContainer(ViewDataDictionary viewData)
      {
        ViewData = viewData;
      }
      public ViewDataDictionary ViewData { get; set; }      
    }
  }
}
