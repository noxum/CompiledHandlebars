using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.Visitors;

namespace CompiledHandlebars.Compiler.AST
{
	internal abstract class BlockWithElse : ASTNode
	{
		protected readonly IList<ASTElementBase> _elseBlock;

		internal readonly bool HasElseBlock;

		internal BlockWithElse(IList<ASTElementBase> children, IList<ASTElementBase> elseBlock, int line, int column)
		  : base(children, line, column)
		{
			HasElseBlock = true;
			_elseBlock = elseBlock;
		}

		internal BlockWithElse(IList<ASTElementBase> children, int line, int column) : base(children, line, column)
		{
			HasElseBlock = false;
		}
	}
}
