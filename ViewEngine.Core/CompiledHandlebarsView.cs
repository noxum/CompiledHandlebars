using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace CompiledHandlebars.ViewEngine.Core
{
	public class CompiledHandlebarsView : IView
	{
		private readonly RenderMethodWrapperBase _funcWrapper;
		private readonly string _path;

		public CompiledHandlebarsView(RenderMethodWrapperBase funcWrapper, string path)
		{
			_path = path;
			_funcWrapper = funcWrapper;
		}

		public string Path => _path;

		public async Task RenderAsync(ViewContext context)
		{			
			var result = await Task.Run(() => _funcWrapper.InvokeRender(context.ViewData?.Model));
			context.Writer.Write(result);
		}
	}
}
