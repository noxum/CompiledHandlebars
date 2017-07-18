using System;
using System.Collections.Generic;
using System.Text;

namespace CompiledHandlebars.ViewEngine.Core
{
	public class CompiledHandlebarsViewEngineOptions
	{
		public IList<string> ViewLocationFormats { get; set; } = new List<string>();
		public IList<string> AreaViewLocationFormats { get; set; } = new List<string>();

		public IEnumerable<string> PossibleVariants(string action, string controller, string area = null)
		{
			if (area!=null)
			{
				foreach (var format in AreaViewLocationFormats)
				{
					yield return String.Format(format, action, controller, area);
				}
			}
			foreach (var format in ViewLocationFormats)
			{
				yield return String.Format(format, action, controller);
			}

		}
	}
}
