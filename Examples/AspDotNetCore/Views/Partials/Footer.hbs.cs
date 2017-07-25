using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using CompiledHandlebars.RuntimeUtils;
using static CompiledHandlebars.RuntimeUtils.RenderHelper;

namespace AspDotNetCore.Views.Partials
{
	[CompiledHandlebarsTemplate]
	public static class Footer
	{
		public static string Render(System.String viewModel)
		{
			var sb = new StringBuilder(64);
			sb.Append("\r\n<h2>This footer is embedded as ");
			sb.Append(WebUtility.HtmlEncode(viewModel));
			sb.Append("!</h2>");
			return sb.ToString();
		}
	}
}