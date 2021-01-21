using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using AspDotNetCore.Views.Partials;
using CompiledHandlebars.RuntimeUtils;
using static CompiledHandlebars.RuntimeUtils.RenderHelper;

namespace AspDotNetCore.Views.Layouts
{
	[CompiledHandlebarsLayout]
	public static class Main
	{
		public static async Task PostRender(System.String viewModel, StringBuilder sb)
		{
			sb.Append("\r\n\t");
			await Footer.RenderAsync("Partial in the layout", sb);
			sb.Append("\r\n</body>\r\n</html>");
		}

		public static async Task PreRender(System.String viewModel, StringBuilder sb)
		{
			sb.Append("\r\n<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title>");
			sb.Append(WebUtility.HtmlEncode(viewModel));
			sb.Append("</title>\r\n</head>\r\n<body>\r\n\t");
		}
	}
}