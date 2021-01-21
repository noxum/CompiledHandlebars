using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspDotNetCore.Views.Layouts;
using CompiledHandlebars.RuntimeUtils;
using static CompiledHandlebars.RuntimeUtils.RenderHelper;

namespace AspDotNetCore.Views
{
	[CompiledHandlebarsTemplate]
	public static class Echo
	{
		public static async Task RenderAsync(System.String viewModel, StringBuilder sb)
		{
			await Main.PreRender(viewModel, sb);
			sb.Append("\r\n<h1>You said ");
			sb.Append(WebUtility.HtmlEncode(viewModel));
			sb.Append("</h1>");
			await Main.PostRender(viewModel, sb);
		}
	}
}