using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetFramework
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine(Template.Render(new ViewModel()
			{
				Name = "Hans Peter"
			}));
		}
	}
}
