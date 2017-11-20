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
		public abstract string InvokeRender(object ViewModel);
	}

	public class StaticRenderMethodWrapper : RenderMethodWrapperBase
	{
		private readonly Func<string> _func;

		public StaticRenderMethodWrapper(MethodInfo renderMethod)
		{
			_func = Delegate.CreateDelegate(typeof(Func<string>), renderMethod) as Func<string>;
		}

		public override string InvokeRender(object ViewModel)
		{
			return _func();
		}
	}

	/// <summary>
	/// A Wrapper around a Function object which can be conveniently created at Runtime through Type.MakeGenericType and Activator.CreateInstance
	/// It is used to do all the naughty reflection stuff in the constructor of the ViewEngine and not sometime else.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class RenderMethodWrapper<T> : RenderMethodWrapperBase where T : class
	{
		private readonly Func<T, string> _func;
		public RenderMethodWrapper(MethodInfo renderMethod)
		{
			_func = Delegate.CreateDelegate(typeof(Func<T, string>), renderMethod) as Func<T, string>;
		}

		public override string InvokeRender(object viewModel)
		{
			return _func.Invoke(viewModel as T);
		}
	}
}
