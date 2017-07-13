using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests
{
	[CompiledHandlebarsLayout]
	public static class LayoutsWithInterfaceModelTest1
	{
		public static string PostRender(CompiledHandlebars.CompilerTests.TestViewModels.IPageModel viewModel)
		{
			var sb = new StringBuilder(64);
			sb.Append("</body>");
			return sb.ToString();
		}

		public static string PreRender(CompiledHandlebars.CompilerTests.TestViewModels.IPageModel viewModel)
		{
			var sb = new StringBuilder(64);
			sb.Append("<head><title>");
			sb.Append(WebUtility.HtmlEncode(viewModel.Title));
			sb.Append("</title></head><body>");
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