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
            StringBuilder sb = new StringBuilder();
            Task.Run(async () => 
                await  Template.RenderAsync(new ViewModel
                {
                    Name = "Hans Peter"
                }, sb)
            ).Wait();

			Console.WriteLine(sb.ToString());
		}
	}
}
