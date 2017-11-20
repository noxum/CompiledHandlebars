using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using AspDotNetCore.Views.Partials;
using CompiledHandlebars.RuntimeUtils;
using static CompiledHandlebars.RuntimeUtils.RenderHelper;

namespace AspDotNetCore.Views.Layouts
{
	[CompiledHandlebarsLayout]
	public static class Main
	{
		public static string PostRender(System.String viewModel)
		{
			var sb = new StringBuilder(64);
			sb.Append("\r\n\t");
			sb.Append(Footer.Render("Partial in the layout"));
			sb.Append("\r\n</body>\r\n</html>");
			return sb.ToString();
		}

		public static string PreRender(System.String viewModel)
		{
			var sb = new StringBuilder(64);
			sb.Append("\r\n<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title>");
			sb.Append(WebUtility.HtmlEncode(viewModel));
			sb.Append("</title>\r\n</head>\r\n<body>\r\n\t");
			return sb.ToString();
		}
	}
}