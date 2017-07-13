using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests
{
	[CompiledHandlebarsTemplate]
	public static class LayoutsWithInterfaceModelTest2
	{
		public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.PageModel viewModel)
		{
			var sb = new StringBuilder(64);
			sb.Append(LayoutsWithInterfaceModelTest1.PreRender(viewModel));
			sb.Append("<h1>");
			sb.Append(WebUtility.HtmlEncode(viewModel.Headline));
			sb.Append("</h1>");
			sb.Append(LayoutsWithInterfaceModelTest1.PostRender(viewModel));
			return sb.ToString();
		}

		private static bool IsTruthy(bool b)
		{
			return b;
		}

		private static bool IsTruthy(string s)
		{
			return !string.IsNullOrEmpty(s);
		}

		private static bool IsTruthy(object o)
		{
			return o != null;
		}

		private static bool IsTruthy<T>(IEnumerable<T> ie)
		{
			return ie != null && ie.Any();
		}

		private static bool IsTruthy(int i)
		{
			return i != 0;
		}

		private class CompiledHandlebarsTemplateAttribute : Attribute
		{
		}

		private class CompiledHandlebarsLayoutAttribute : Attribute
		{
		}
	}
}/**/