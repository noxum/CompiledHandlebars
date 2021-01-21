using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.RuntimeUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;

namespace CompiledHandlebars.ViewEngine.Core
{
    public class CompiledHandlebarsViewEngine : IViewEngine
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private Dictionary<string, RenderMethodWrapperBase> _mappings { get; set; } = new Dictionary<string, RenderMethodWrapperBase>();
        private readonly CompiledHandlebarsViewEngineOptions _options;
        private const string AreaKey = "area";
        private const string ControllerKey = "controller";

        public static void Initialize(IServiceProvider services)
        {
            _httpContextAccessor = services.GetRequiredService<IHttpContextAccessor>();
        }

        public CompiledHandlebarsViewEngine(Assembly assembly, CompiledHandlebarsViewEngineOptions options)
        {
            _options = options;

            //The basic idea: Search through the assembly, find all compiled HandlebarsTemplates and create Func<>-objects for their render methods.
            //Wrap these Func<>-objects in a generic class and store it in a Dictionary to be able to access them at runtime            
            //Then the View can call the wrapped Func<>-object without knowing the actual type of the viewModel and without using reflection!
            foreach (var template in assembly.GetTypes().Where(x => x.GetTypeInfo().GetCustomAttribute(typeof(CompiledHandlebarsTemplateAttribute), false) != null))
            {
                var renderMethod = template.GetMethod("RenderAsync");
                Type wrapperType;

                if (renderMethod.ReturnType != typeof(Task))
                {
                    continue;
                }

                var parameters = renderMethod.GetParameters().ToArray();

                if (parameters.Length == 0)
                {
                    continue;
                }

                if (parameters.Length == 1 && parameters[0].ParameterType == typeof(StringBuilder))
                {
                    wrapperType = typeof(StaticRenderMethodWrapper);
                }
                else if (parameters.Length == 2 && parameters[1].ParameterType == typeof(StringBuilder))
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
            var assemblyName = assembly.GetName();
            int indexOfViews = template.Namespace?.IndexOf(".Views.", StringComparison.InvariantCultureIgnoreCase) ?? -1;

            //Stiwa-Spezifisch
            if (assemblyName.Name == "Website" && indexOfViews >= 0)
            {
                return string.Concat("~/", template.Namespace.Substring(indexOfViews + 1).Replace('.', '/'), "/", template.Name.Replace('.', '/'), ".hbs");
            }

            if (assemblyName.Name.Length == template.Namespace.Length)
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
            string controllerName = RazorViewEngine.GetNormalizedRouteValue(context, ControllerKey);
            string areaName = RazorViewEngine.GetNormalizedRouteValue(context, AreaKey);

            string match = _options.PossibleVariants(viewName, controllerName, areaName).FirstOrDefault(x => _mappings.ContainsKey(x.ToLower()));
            if (!String.IsNullOrEmpty(match))
            {
                isMainPage = CheckIsMainPage(isMainPage);
                return ViewEngineResult.Found(viewName, new CompiledHandlebarsView(_mappings[match.ToLower()], match, isMainPage));
            }
            else
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
            if (_mappings.TryGetValue(viewPath.ToLower(), out var view))
            {
                isMainPage = CheckIsMainPage(isMainPage);

                return ViewEngineResult.Found(viewPath, new CompiledHandlebarsView(view, viewPath, isMainPage));
            }
            else
            {
                return ViewEngineResult.NotFound(viewPath, new string[] { viewPath });
            }
        }

        /// <summary>
        /// When the request is an Action which returns a ViewComponent the isMainPage parameter is not set.
        /// This code circumwents this bug even for child ViewComponents.
        /// The View thats is searched first sets its "isMainPage" for all others.
        /// </summary>
        /// <param name="isMainPage"></param>
        /// <returns></returns>
        private bool CheckIsMainPage(bool isMainPage)
        {
            HttpContext httpContext = _httpContextAccessor.HttpContext;

            if (isMainPage)
            {
                httpContext.Items["isMainPage"] = true;
            }
            else
            {
                if (httpContext.Items["isMainPage"] == null)
                {
                    isMainPage = true;
                    httpContext.Items["isMainPage"] = true;
                }
            }

            return isMainPage;
        }
    }
}
