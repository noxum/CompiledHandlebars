using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.TestViewModels
{
	public class LinkModel
	{
		public string Url { get; set; }
		public string Text { get; set; }
		public string Title { get; set; }
		public bool IsActive { get; set; }
		public bool IsDisabled { get; set; }
		public bool NoFollow { get; set; }
		public bool Popup { get; set; }
	}
}
