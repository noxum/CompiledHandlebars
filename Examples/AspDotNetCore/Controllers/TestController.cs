using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspDotNetCore.Controllers
{
    public class TestController : Controller
    {

		public IActionResult Index()
		{
			return View("~/Template.hbs", new ViewModel() { Name = "HansPeter" });
		}

		public IActionResult Greet(string name)
		{
			return View("Greet.hbs", new ViewModel() { Name = name });
		}

		public IActionResult echo(int num)
		{
			return View("Echo.hbs", num);
		}
    }
}
