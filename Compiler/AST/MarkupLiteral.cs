using System;
using CompiledHandlebars.Compiler.Visitors;

namespace CompiledHandlebars.Compiler.AST
{
	internal class MarkupLiteral : ASTElementBase
	{
		internal readonly string Value;

		internal MarkupLiteral(string value, int line, int column) : base(line, column)
		{
			Value = value;
		}

		internal MarkupLiteral(string value, bool trimLeft, bool trimRight, int line, int column) : base(line, column)
		{
			var tmpVal = value;
			if (trimLeft)
				tmpVal = tmpVal.TrimStart(' ', '\n', '\r', '\t');
			if (trimRight)
				tmpVal = tmpVal.TrimEnd(' ', '\n', '\r', '\t');
			Value = tmpVal;
		}

		internal override void Accept(IASTVisitor visitor)
		{
			visitor.Visit(this);
		}

		internal override bool HasExpressionOnLoopLevel<T>()
		{
			return false;
		}
	}
}
