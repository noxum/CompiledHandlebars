using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.AST.Expressions;
using CompiledHandlebars.Compiler.Visitors;

namespace CompiledHandlebars.Compiler.AST
{
	internal class EqualsExpression
	{
		internal readonly Expression Lhs;
		internal readonly Expression Rhs;

		internal EqualsExpression(Expression lhs, Expression rhs)
		{
			Lhs = lhs;
			Rhs = rhs;
		}

		internal bool HasExpressionOnLoopLevel<T>()
		{
			if (Lhs is T || Rhs is T)
			{
				return true;
			}
			return false;
		}
	}

	internal class EqualsBlock : BlockWithElse
	{
		internal readonly EqualsExpression Expr;

		internal EqualsBlock(EqualsExpression expr, IList<ASTElementBase> children, int line, int column) 
			: base(children, line, column)
		{
			Expr = expr;
		}

		internal EqualsBlock(EqualsExpression expr, IList<ASTElementBase> children, IList<ASTElementBase> elseBlock, int line, int column) 
			: base(children, elseBlock, line, column)
		{
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
			if (Expr.HasExpressionOnLoopLevel<T>())
				return true;
			if (HasElseBlock)
				return _elseBlock.Any(x => x.HasExpressionOnLoopLevel<T>()) || _children.Any(x => x.HasExpressionOnLoopLevel<T>());
			else
				return _children.Any(x => x.HasExpressionOnLoopLevel<T>());
		}
	}
}
