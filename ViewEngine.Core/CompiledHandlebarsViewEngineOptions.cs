using System;
using System.Collections.Generic;
using System.Text;

namespace CompiledHandlebars.ViewEngine.Core
{
	public class CompiledHandlebarsViewEngineOptions
	{
		/// <summary>
		/// Possible locations for views.
		/// Format strings may contain the ViewName '{0}' and the ControllerName '{1}'
		/// </summary>
		public IList<string> ViewLocationFormats { get; set; } = new List<string>();
		/// <summary>
		/// Possible locations for view
		/// Format strings may contain the ViewName '{0}', the ControllerName '{1}' and the AreaName '{2}'
		/// </summary>
		public IList<string> AreaViewLocationFormats { get; set; } = new List<string>();

		/// <summary>
		/// Generate all possible view locations from the ViewLocationFormats and the passed parameters
		/// </summary>
		/// <param name="action"></param>
		/// <param name="controller"></param>
		/// <param name="area"></param>
		/// <returns></returns>
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
