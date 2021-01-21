using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.DependencyInjection;

namespace CompiledHandlebars.ViewEngine.Core
{
    public class CompiledHandlebarsView : IView
    {
        private readonly RenderMethodWrapperBase _funcWrapper;
        private readonly bool _isMainPage;
        private readonly string _path;

        public CompiledHandlebarsView(RenderMethodWrapperBase funcWrapper, string path, bool isMainPage)
        {
            _path = path;
            _funcWrapper = funcWrapper;
            _isMainPage = isMainPage;
        }

        public string Path => _path;

        public async Task RenderAsync(ViewContext context)
        {
            context.HttpContext.Items["_viewcontext"] = context;

            StringBuilder sb = new StringBuilder(4096);
            await _funcWrapper.InvokeRender(context.ViewData?.Model, sb);
            string result = sb.ToString();

            if (_isMainPage)
            {
                StringFilterStream stringFilterStream = context.HttpContext.RequestServices.GetRequiredService<StringFilterStream>();
                if (stringFilterStream.Active)
                {
                    result = stringFilterStream.Process(result);
                }
            }

            await context.Writer.WriteAsync(result);
        }
    }
}
