using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using CompiledHandlebars.RuntimeUtils;
using Microsoft.AspNetCore.Mvc.Razor;

namespace CompiledHandlebars.ViewEngine.Core
{
	public class CompiledHandlebarsViewEngine : IViewEngine
	{
		private Dictionary<string, RenderMethodWrapperBase> _mappings { get; set; } = new Dictionary<string, RenderMethodWrapperBase>();
		private readonly CompiledHandlebarsViewEngineOptions _options;

		private const string AreaKey = "area";
		private const string ControllerKey = "controller";		

		public CompiledHandlebarsViewEngine(Assembly assembly, CompiledHandlebarsViewEngineOptions options)
		{
			_options = options;

			//The basic idea: Search through the assembly, find all compiled HandlebarsTemplates and create Func<>-objects for their render methods.
			//Wrap these Func<>-objects in a generic class and store it in a Dictionary to be able to access them at runtime            
			//Then the View can call the wrapped Func<>-object without knowing the actual type of the viewModel and without using reflection!
			foreach (var template in assembly.GetTypes().Where(x => x.GetTypeInfo().GetCustomAttribute(typeof(CompiledHandlebarsTemplateAttribute), false) != null))
			{
				var renderMethod = template.GetMethod("Render");
				Type wrapperType;
				if (!renderMethod.GetParameters().Any())
				{
					wrapperType = typeof(StaticRenderMethodWrapper);
				}
				else if (renderMethod.GetParameters().Count() == 1)
				{
					wrapperType = typeof(RenderMethodWrapper<>).MakeGenericType(renderMethod.GetParameters().First().ParameterType);
				}
				else
				{
					continue;
				}
				_mappings.Add(GetVirtualPath(assembly, template).ToLower(), Activator.CreateInstance(wrapperType, renderMethod) as RenderMethodWrapperBase);
			}
		}
		
		/// <summary>
		/// Determines the VirtualPath for a Template.
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="template"></param>
		/// <returns></returns>
		private string GetVirtualPath(Assembly assembly, Type template)
		{
			if (assembly.GetName().Name.Length == template.Namespace.Length)
			{
				return string.Concat("~/", template.Name.Replace('.', '/'), ".hbs");
			}
			else
			{
				return string.Concat("~/", template.Namespace.Substring(assembly.GetName().Name.Length + 1).Replace('.', '/'), "/", template.Name.Replace('.', '/'), ".hbs");
			}
		}

		/// <summary>
		/// Tries to find a View by name and context (controller and area name)
		/// It uses the viewlocation formats passed in the options to generate possible locations and then looks in the mapping if they exist
		/// First match wins!
		/// </summary>
		/// <param name="context"></param>
		/// <param name="viewName"></param>
		/// <param name="isMainPage"></param>
		/// <returns></returns>
		public ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage)
		{
			var controllerName = RazorViewEngine.GetNormalizedRouteValue(context, ControllerKey);
			var areaName = RazorViewEngine.GetNormalizedRouteValue(context, AreaKey);

			var match = _options.PossibleVariants(viewName, controllerName, areaName).FirstOrDefault(x => _mappings.ContainsKey(x.ToLower()));
			if (!String.IsNullOrEmpty(match)) {
				return ViewEngineResult.Found(viewName, new CompiledHandlebarsView(_mappings[match.ToLower()], match));
			} else
			{
				return ViewEngineResult.NotFound(viewName, _options.PossibleVariants(viewName, controllerName, areaName));
			}
		}

		/// <summary>
		/// Tries to find a View by its ViewPath.
		/// Checks if the ViewPath exists in the mappings and then returns the View
		/// </summary>
		/// <param name="executingFilePath"></param>
		/// <param name="viewPath"></param>
		/// <param name="isMainPage"></param>
		/// <returns></returns>
		public ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage)
		{
			if (_mappings.ContainsKey(viewPath.ToLower()))
			{
				return ViewEngineResult.Found(viewPath, new CompiledHandlebarsView(_mappings[viewPath.ToLower()], viewPath));
			} else
			{
				return ViewEngineResult.NotFound(viewPath, new string[] { viewPath});
			}
		}
	}
}
