using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspDotNetCore.Views.Layouts;
using AspDotNetCore.Views.Partials;
using CompiledHandlebars.RuntimeUtils;
using static CompiledHandlebars.RuntimeUtils.RenderHelper;

namespace AspDotNetCore.Views
{
	[CompiledHandlebarsTemplate]
	public static class Greet
	{
		public static async Task RenderAsync(System.String viewModel, StringBuilder sb)
		{
			await Main.PreRender(viewModel, sb);
			sb.Append("\r\n\t<h1>Hello ");
			sb.Append(WebUtility.HtmlEncode(viewModel));
			sb.Append("</h1>\r\n");
			await Footer.RenderAsync("Partial", sb);
			await Main.PostRender(viewModel, sb);
		}
	}
}