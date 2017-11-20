using System;

namespace DotNetCore
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