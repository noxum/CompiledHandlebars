using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.TestViewModels
{
	public class SelfReferencingViewModel
	{
		public string Name { get; set; }
		public SelfReferencingViewModel Child { get; set; }
	}

	public static class SelfReferencingViewModelFactory
	{
		public static SelfReferencingViewModel Create()
		{
			return new SelfReferencingViewModel()
			{
				Name = "Parent",
				Child = new SelfReferencingViewModel()
				{
					Name = "Child"
				}
			};
		}
	}
}
