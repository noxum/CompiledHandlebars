using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.ViewEngine
{
    public abstract class RenderMethodWrapperBase
    {
        public abstract Task InvokeRender(object ViewModel, StringBuilder sb);
    }

    public class StaticRenderMethodWrapper : RenderMethodWrapperBase
    {
        private readonly Func<StringBuilder, Task> _func;

        public StaticRenderMethodWrapper(MethodInfo renderMethod)
        {
            _func = renderMethod.CreateDelegate(typeof(Func<StringBuilder, Task>)) as Func<StringBuilder, Task>;
        }

        public override Task InvokeRender(object ViewModel, StringBuilder sb)
        {
            return _func(sb);
        }
    }

    /// <summary>
    /// A Wrapper around a Function object which can be conveniently created at Runtime through Type.MakeGenericType and Activator.CreateInstance
    /// It is used to do all the naughty reflection stuff in the constructor of the ViewEngine and not sometime else.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RenderMethodWrapper<T> : RenderMethodWrapperBase where T : class
    {
        private readonly Func<T, StringBuilder, Task> _func;

        public RenderMethodWrapper(MethodInfo renderMethod)
        {
            _func = renderMethod.CreateDelegate(typeof(Func<T, StringBuilder, Task>)) as Func<T, StringBuilder, Task>;
        }

        public override Task InvokeRender(object viewModel, StringBuilder sb)
        {
            return _func.Invoke(viewModel as T, sb);
        }
    }
}
