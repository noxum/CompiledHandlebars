using CompiledHandlebars.RuntimeUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace CompiledHandlebars.ViewEngine
{

    public class CompiledHandlebarsViewEngine : VirtualPathProviderViewEngine
	{
		private Dictionary<string, RenderMethodWrapperBase> _mappings { get; set; } = new Dictionary<string, RenderMethodWrapperBase>();
		public CompiledHandlebarsViewEngine(Assembly assembly)
		{
			//Defaults. Must be set, can be overwritten
			ViewLocationFormats = new string[] { "~/Views/{1}/{0}.hbs", "~/Views/Shared/{0}.hbs" };
			PartialViewLocationFormats = new string[] { "~/Views/{1}/{0}.hbs", "~/Views/Shared/{0}.hbs" };

			//This part is close to magic.
			//The basic idea: Search through the assembly, find all compiled HandlebarsTemplates and create Func<>-objects for their render methods.
			//Wrap these Func<>-objects in a generic class and store it in a Dictionary to be able to access them at runtime            
			//Then the View can call the wrapped Func<>-object without knowing the actual type of the viewModel and without using reflection!
			foreach (var template in assembly.GetTypes().Where(x => x.GetCustomAttribute(typeof(CompiledHandlebarsTemplateAttribute), false) != null))
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
			return string.Concat("~/", template.Namespace.Substring(assembly.GetName().Name.Length + 1).Replace('.', '/'), "/", template.Name.Replace('.', '/'), ".hbs");
		}

		protected override IView CreatePartialView(ControllerContext controllerContext, string partialPath)
		{
			partialPath = EnsureVirtualPathPrefixToLower(partialPath);
            if(_mappings.TryGetValue(partialPath, out var partial))
				return new CompiledHandlebarsView(partial);
			return null;
		}

		protected override IView CreateView(ControllerContext controllerContext, string viewPath, string masterPath)
		{
			viewPath = EnsureVirtualPathPrefixToLower(viewPath);
            if (_mappings.TryGetValue(viewPath, out var view))
                return new CompiledHandlebarsView(view);
			return null;
		}

		private static string EnsureVirtualPathPrefixToLower(string virtualPath)
		{
			if (!String.IsNullOrEmpty(virtualPath))
			{
				// For a virtual path lookups to succeed, it needs to start with a ~/.
				if (!virtualPath.StartsWith("~/", StringComparison.Ordinal))
				{
					virtualPath = "~/" + virtualPath.TrimStart(new[] { '/', '~' });
				}
			}
			return virtualPath.ToLower();
		}

		protected override bool FileExists(ControllerContext controllerContext, string virtualPath)
		{
			return _mappings.ContainsKey(virtualPath);
		}

		public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
		{
			var result = base.FindPartialView(controllerContext, partialViewName, useCache);
			if (result.View != null)
				return result;
			foreach (var loc in result.SearchedLocations)
			{
				if (_mappings.ContainsKey(loc.ToLower()))
					return new ViewEngineResult(CreatePartialView(controllerContext, loc), this);
			}
			return result;
		}

		public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
		{
			var result = base.FindView(controllerContext, viewName, masterName, useCache);
			if (result.View != null)
				return result;
			foreach (var loc in result.SearchedLocations)
			{
				if (_mappings.ContainsKey(loc.ToLower()))
					return new ViewEngineResult(CreatePartialView(controllerContext, loc), this);
			}
			return result;
		}
	}
}
