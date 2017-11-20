using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CompiledHandlebars.Compiler.AST.Expressions
{
	internal class MemberExpression : Expression
	{
		public readonly IdentifierElement Path;
		internal MemberExpression(IdentifierElement path)
		{
			Path = path;
		}

		internal override bool TryEvaluate(CompilationState state, out Context context)
		{
			//Copy Stack as identifier elements manipulate (push, pop)
			var cpContextStack = new Stack<Context>();
			cpContextStack = new Stack<Context>(state.ContextStack.Reverse());
			return Path.TryEvaluate(cpContextStack, state, out context);
		}

		/// <summary>
		/// Will evaluate to a context inside a loop. Used for the context inside #each blocks
		/// </summary>
		/// <param name="state"></param>
		/// <returns></returns>
		internal Context EvaluateLoop(CompilationState state)
		{
			if (TryEvaluate(state, out Context loopVariable))
			{
				var elementSymbol = loopVariable.Symbol.GetElementSymbol();
				if (elementSymbol != null)
					return state.BuildLoopContext(loopVariable.Symbol.GetElementSymbol());
			}
			return null;
		}

		public override string ToString()
		{
			return Path.ToString();
		}


	}
}
