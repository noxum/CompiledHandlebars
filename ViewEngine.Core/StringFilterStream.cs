using System;
using System.Collections.Generic;
using System.Text;
using CompiledHandlebars.ViewEngine.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CompiledHandlebars.ViewEngine.Core
{
    public class StringFilterStream
    {
        private readonly List<FilterItem> _filterList;
        private readonly ILogger<StringFilterStream> _logger;

        public bool Active { get; private set; }

        public StringFilterStream(ILogger<StringFilterStream> logger)
        {
            _filterList = new List<FilterItem>();
            _logger = logger;
        }

        public void AddFilter(string name, Func<string, string> func)
        {
            Active = true;
            _filterList.Add(new FilterItem { Name = name, Function = func });
        }

        public string Process(string s)
        {
            foreach (var filter in _filterList)
            {
                _logger.LogTrace($"{filter.Name} start");
                s = filter.Function(s);
            }
            return s;
        }

        private class FilterItem
        {
            public String Name { get; set; }
            public Func<string, string> Function { get; set; }
        }
    }
}
namespace Microsoft.Extensions.DependencyInjection
{
    public static class StringFilterStreamExtensions
    {
        public static IServiceCollection AddStringFilterStream(this IServiceCollection services)
        {
            return services.AddScoped<StringFilterStream>();
        }
    }
}
