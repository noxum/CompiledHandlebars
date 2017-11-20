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

		[Route("Greet/{name}")]
		public IActionResult Greet(string name)
		{	
			return View("Greet", name);
		}

		[Route("Echo/{num}")]
		public IActionResult echo(int num)
		{
			return View("Echo", num.ToString());
		}
    }
}
