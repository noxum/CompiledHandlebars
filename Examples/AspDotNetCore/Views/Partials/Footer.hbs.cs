using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using CompiledHandlebars.RuntimeUtils;
using static CompiledHandlebars.RuntimeUtils.RenderHelper;

namespace AspDotNetCore.Views.Partials
{
	[CompiledHandlebarsTemplate]
	public static class Footer
	{
		public static async Task RenderAsync(System.String viewModel, StringBuilder sb)
		{
			sb.Append("\r\n<h2>This footer is embedded as ");
			sb.Append(WebUtility.HtmlEncode(viewModel));
			sb.Append("!</h2>");
		}
	}
}