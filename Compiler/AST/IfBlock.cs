using CompiledHandlebars.Compiler.AST.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.Visitors;

namespace CompiledHandlebars.Compiler.AST
{
	internal class IfBlock : BlockWithElse
	{
		internal readonly Expression Expr;
		internal readonly IfType QueryType;

		internal IfBlock(Expression member, IfType type, IList<ASTElementBase> children, int line, int column)
							 : base(children, line, column)
		{
			QueryType = type;
			Expr = member;
		}

		internal IfBlock(Expression expr, IfType type, IList<ASTElementBase> elseBlock, IList<ASTElementBase> children, int line, int column)
							 : base(children, elseBlock, line, column)
		{
			QueryType = type;
			Expr = expr;
		}


		internal override void Accept(IASTVisitor visitor)
		{
			visitor.VisitEnter(this);
			foreach (var child in _children)
				child.Accept(visitor);
			if (HasElseBlock)
			{
				visitor.VisitElse(this);
				foreach (var ele in _elseBlock)
				{
					ele.Accept(visitor);
				}
			}
			visitor.VisitLeave(this);
		}

		internal override bool HasExpressionOnLoopLevel<T>()
		{
			if (Expr is T)
				return true;
			if (HasElseBlock)
				return _elseBlock.Any(x => x.HasExpressionOnLoopLevel<T>()) || _children.Any(x => x.HasExpressionOnLoopLevel<T>());
			else
				return _children.Any(x => x.HasExpressionOnLoopLevel<T>());
		}
	}

	internal enum IfType { If, Unless }
}
