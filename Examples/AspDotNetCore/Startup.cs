using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CompiledHandlebars.ViewEngine.Core;
using System.Reflection;
using CompiledHandlebars.RuntimeUtils;

namespace AspDotNetCore
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			var ve_options = new CompiledHandlebarsViewEngineOptions()
			{
				ViewLocationFormats = new string[] { "~/Views/{1}/{0}.hbs", "~/Views/{0}.hbs", "~/{0}.hbs" }
			};

			services.AddMvc().AddViewOptions(options =>
				{
					options.ViewEngines.Clear();
					options.ViewEngines.Add(new CompiledHandlebarsViewEngine(typeof(Startup).GetTypeInfo().Assembly, ve_options));
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole();

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					 name: "default",
					 template: "{controller=Test}/{action=Index}/{id?}");
			});
		}

		[CompiledHandlebarsHelperMethod]
		public static string TestHelper(ViewModel vm)
		{
			return $"ViewModel(Name:{vm.Name})";
		}
	}
}
