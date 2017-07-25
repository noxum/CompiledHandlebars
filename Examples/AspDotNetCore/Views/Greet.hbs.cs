using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using AspDotNetCore.Views.Layouts;
using AspDotNetCore.Views.Partials;
using CompiledHandlebars.RuntimeUtils;
using static CompiledHandlebars.RuntimeUtils.RenderHelper;

namespace AspDotNetCore.Views
{
	[CompiledHandlebarsTemplate]
	public static class Greet
	{
		public static string Render(System.String viewModel)
		{
			var sb = new StringBuilder(64);
			sb.Append(Main.PreRender(viewModel));
			sb.Append("\r\n\t<h1>Hello ");
			sb.Append(WebUtility.HtmlEncode(viewModel));
			sb.Append("</h1>\r\n");
			sb.Append(Footer.Render("Partial"));
			sb.Append(Main.PostRender(viewModel));
			return sb.ToString();
		}
	}
}