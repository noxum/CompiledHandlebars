using System;
using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;
using Microsoft.CodeAnalysis;

namespace CompiledHandlebars.Compiler.AST.Expressions
{

	internal abstract class SpecialExpression : Expression
	{
		protected bool InsideEachLoopCheck(CompilationState state)
		{
			if (state.LoopLevel > 0)
				return true;
			state.AddTypeError("SpecialExpressions can only exist inside EachBlocks", HandlebarsTypeErrorKind.SpecialExpressionOutsideEachLoop);
			return false;
		}

		protected CompileTimeLoopContext IsCompileTimeLoop(CompilationState state)
		{
			return state.ContextStack.Peek() as CompileTimeLoopContext;
		}
	}

	internal class FirstExpression : SpecialExpression
	{
		internal override bool TryEvaluate(CompilationState state, out Context context)
		{
			var ctLoopContext = IsCompileTimeLoop(state);
			if (ctLoopContext != null)
			{//Inside CTLoop -> Return literal
				context = new Context(ctLoopContext.First.ToString().ToLower(), state.Introspector.GetBoolTypeSymbol());
				return true;
			}
			else
			{
				context = new Context($"first{state.LoopLevel}", state.Introspector.GetBoolTypeSymbol());
				return InsideEachLoopCheck(state);
			}
		}
	}

	internal class LastExpression : SpecialExpression
	{

		internal override bool TryEvaluate(CompilationState state, out Context context)
		{
			var ctLoopContext = IsCompileTimeLoop(state);
			if (ctLoopContext != null)
			{//Inside CTLoop -> Return literal
				context = new Context(ctLoopContext.Last.ToString().ToLower(), state.Introspector.GetBoolTypeSymbol());
				return true;
			}
			else
			{
				context = new Context($"last{state.LoopLevel}", state.Introspector.GetBoolTypeSymbol());
				return InsideEachLoopCheck(state);
			}
		}
	}

	internal class IndexExpression : SpecialExpression
	{

		internal override bool TryEvaluate(CompilationState state, out Context context)
		{
			var ctLoopContext = IsCompileTimeLoop(state);
			if (ctLoopContext != null)
			{//Inside CTLoop -> Return literal
				context = new Context(ctLoopContext.Index.ToString(), state.Introspector.GetIntTypeSymbol());
				return true;
			}
			else
			{
				context = new Context($"index{state.LoopLevel}", state.Introspector.GetIntTypeSymbol());
				return InsideEachLoopCheck(state);
			}
		}
	}

	internal class KeyExpression : SpecialExpression
	{
		internal override bool TryEvaluate(CompilationState state, out Context context)
		{
			var ctLoopContext = IsCompileTimeLoop(state);
			if (ctLoopContext != null)
			{
				context = new Context($"\"{ctLoopContext.Key}\"", state.Introspector.GetStringTypeSymbol());
				return true;
			}
			else
			{
				state.AddTypeError("KeyExpression outside a compiletime loop over an Object", HandlebarsTypeErrorKind.IllegalKeyExpression);
				context = null;
				return false;
			}
		}
	}


}
