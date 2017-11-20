using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using AspDotNetCore.Views.Layouts;
using CompiledHandlebars.RuntimeUtils;
using static CompiledHandlebars.RuntimeUtils.RenderHelper;

namespace AspDotNetCore.Views
{
	[CompiledHandlebarsTemplate]
	public static class Echo
	{
		public static string Render(System.String viewModel)
		{
			var sb = new StringBuilder(64);
			sb.Append(Main.PreRender(viewModel));
			sb.Append("\r\n<h1>You said ");
			sb.Append(WebUtility.HtmlEncode(viewModel));
			sb.Append("</h1>");
			sb.Append(Main.PostRender(viewModel));
			return sb.ToString();
		}
	}
}