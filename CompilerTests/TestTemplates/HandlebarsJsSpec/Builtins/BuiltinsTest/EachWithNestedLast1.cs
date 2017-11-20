using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Collections.Generic;

namespace CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins
{
	[CompiledHandlebarsTemplate]
	public static class EachWithNestedLast1
	{
		public static string Render(CompiledHandlebars.CompilerTests.HandlebarsJsSpec.Builtins.TextListModel viewModel)
		{
			var sb = new StringBuilder(64);
			if (IsTruthy(viewModel) && IsTruthy(viewModel.Goodbyes))
			{
				int index1 = 0;
				bool last1 = false;
				foreach (var loopItem0 in viewModel.Goodbyes)
				{
					last1 = index1 == (viewModel.Goodbyes.Count() - 1);
					sb.Append("(");
					if (IsTruthy(last1))
					{
						sb.Append(WebUtility.HtmlEncode(loopItem0.Text));
						sb.Append("! ");
					}

					int index2 = 0;
					bool last2 = false;
					foreach (var loopItem1 in viewModel.Goodbyes)
					{
						last2 = index2 == (viewModel.Goodbyes.Count() - 1);
						if (IsTruthy(last2))
						{
							sb.Append(WebUtility.HtmlEncode(loopItem1.Text));
							sb.Append("!");
						}

						index2++;
					}

					if (IsTruthy(last1))
					{
						sb.Append(" ");
						sb.Append(WebUtility.HtmlEncode(loopItem0.Text));
						sb.Append("!");
					}

					sb.Append(") ");
					index1++;
				}
			}

			sb.Append("cruel ");
			sb.Append(WebUtility.HtmlEncode(viewModel.World));
			sb.Append("!");
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