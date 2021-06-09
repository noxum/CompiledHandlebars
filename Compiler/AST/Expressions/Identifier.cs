using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;
using System.Collections.Generic;
using System.Linq;
using System;

namespace CompiledHandlebars.Compiler.AST.Expressions
{

	internal class RootIdentifier : IdentifierElement
	{
		internal RootIdentifier(IdentifierElement next) : base(next) { }

		internal override bool TryEvaluate(Stack<Context> contextStack, CompilationState state, out Context context)
		{
			if (_next == null)
			{
				context = contextStack.Last();
				return true;
			}
			else
			{
				var rootedContextStack = new Stack<Context>();
				rootedContextStack.Push(contextStack.Last());
				return _next.TryEvaluate(rootedContextStack, state, out context);
			}
		}
	}

	internal class ThisIdentifier : IdentifierElement
	{

		internal ThisIdentifier(IdentifierElement next) : base(next) { }

		internal override bool TryEvaluate(Stack<Context> contextStack, CompilationState state, out Context context)
		{
			if (_next == null)
			{
				context = contextStack.Peek();
				return true;
			}
			else
				return _next.TryEvaluate(contextStack, state, out context);
		}
	}
	internal class Identifier : IdentifierElement
	{
		private readonly string _value;

		internal Identifier(string value, IdentifierElement next) : base(next)
		{
			_value = value;
		}

		internal override bool TryEvaluate(Stack<Context> contextStack, CompilationState state, out Context context)
		{
			//Add the Identifier to the current context
			var memberSymbol = contextStack.Any() ? contextStack.Peek().Symbol.FindMember(_value) :
																 state.Introspector.GetTypeSymbol(_value);
			if (memberSymbol != null)
			{
				var identifierContext = new Context(string.Join(".", contextStack.Peek().FullPath, _value), memberSymbol);
				if (_next == null)
				{
					//Last element => return IdentifierContext
					context = identifierContext;
					return true;
				}
				else
				{
					//Push the identifier on the contextStack and keep going
					contextStack.Push(identifierContext);
					return _next.TryEvaluate(contextStack, state, out context);
				}
			}
			else
			{
				state.AddTypeError($"Could not find Member '{_value}' in Type '{contextStack.Peek().Symbol.ToDisplayString()}'!", HandlebarsTypeErrorKind.UnknownMember, $"{state.Template.Namespace}.{state.Template.Name}|{_value}");
				context = null;
				return false;
			}
		}

		public override string ToString()
		{
			if (_next != null)
				return string.Join(".", _value, _next.ToString());
			return _value;
		}


	}

	internal class PathUp : IdentifierElement
	{
		internal PathUp(IdentifierElement next) : base(next) { }

		internal override bool TryEvaluate(Stack<Context> contextStack, CompilationState state, out Context context)
		{
			if (!contextStack.Any() || contextStack.Count == 1)
			{
				state.AddTypeError("Error in MemberExpression: Empty ContextStack but PathUp Element ('../')!", HandlebarsTypeErrorKind.EmptyContextStack);
				context = null;
				return false;
			}
			contextStack.Pop();
			return _next.TryEvaluate(contextStack, state, out context);
		}

		public override string ToString()
		{
			return string.Concat("../", _next.ToString());
		}


	}


	/// <summary>
	/// Represents an identifier inside Handlebars as a linked list of IdentifierElements
	/// IdentifierElements are either
	///   Identifier 
	///   PathUp ("../")
	/// 
	///Identifier seperators (".","/") can be ignored as their semantic value is represented as seperated Identifier objects
	/// 
	/// ../A.B => PathUp->Identifier(A)->Identifier(B)
	/// 
	/// </summary>
	internal abstract class IdentifierElement
	{
		protected readonly IdentifierElement _next;

		internal IdentifierElement(IdentifierElement next)
		{
			_next = next;
		}

		internal abstract bool TryEvaluate(Stack<Context> contextStack, CompilationState state, out Context context);

	}
}
