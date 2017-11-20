using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests
{
	[CompiledHandlebarsTemplate]
	public static class BasicEqualsTest3
	{
		public static string Render(CompiledHandlebars.CompilerTests.TestViewModels.MarsModel viewModel)
		{
			var sb = new StringBuilder(64);
			if ((viewModel.Name == null && "Mars" == null) || (viewModel.Name != null && "Mars" != null && viewModel.Name.Equals("Mars")))
			{
				sb.Append("EQUAL3");
			}
			else
			{
				sb.Append("NOTEQUAL3");
			}

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