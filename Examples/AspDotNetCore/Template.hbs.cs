using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompiledHandlebars.RuntimeUtils;
using static CompiledHandlebars.RuntimeUtils.RenderHelper;

namespace AspDotNetCore
{
	[CompiledHandlebarsTemplate]
	public static class Template
	{
		public static async Task RenderAsync(AspDotNetCore.ViewModel viewModel, StringBuilder sb)
		{
			sb.Append("\r\n<html>\r\n<body>\r\n\t<h1>Hello ");
			sb.Append(WebUtility.HtmlEncode(viewModel.Name));
			sb.Append("!</h1>\r\n\t<p>");
			sb.Append(WebUtility.HtmlEncode(Startup.TestHelper(viewModel)));
			sb.Append("</p>\r\n</body>\r\n</html>");
		}
	}
}