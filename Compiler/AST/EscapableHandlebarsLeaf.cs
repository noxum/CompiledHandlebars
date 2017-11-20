using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler.AST
{
	internal abstract class EncodableHandlebarsLeaf : ASTElementBase
	{

		internal EncodableHandlebarsLeaf(int line, int column) : base(line, column) { }

		internal TokenType Type { get; set; } = TokenType.Encoded;

		internal void SetTokenType(TokenType type)
		{
			Type = type;
		}
	}

	internal enum TokenType { Undefined, Encoded, Unencoded }

}
