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
      string output = (string)renderMethod.Invoke(null, new object[1] { viewContext.ViewData?.Model });
      writer.Write(output);
    }  
  }
}
