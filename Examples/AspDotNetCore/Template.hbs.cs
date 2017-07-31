using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using CompiledHandlebars.RuntimeUtils;
using static CompiledHandlebars.RuntimeUtils.RenderHelper;

namespace AspDotNetCore
{
	[CompiledHandlebarsTemplate]
	public static class Template
	{
		public static string Render(AspDotNetCore.ViewModel viewModel)
		{
			var sb = new StringBuilder(64);
			sb.Append("\r\n<html>\r\n<body>\r\n\t<h1>Hello ");
			sb.Append(WebUtility.HtmlEncode(viewModel.Name));
			sb.Append("!</h1>\r\n\t<p>");
			sb.Append(WebUtility.HtmlEncode(Startup.TestHelper(viewModel)));
			sb.Append("</p>\r\n</body>\r\n</html>");
			return sb.ToString();
		}
	}
}