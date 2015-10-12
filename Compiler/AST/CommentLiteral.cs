using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.Visitors;

namespace CompiledHandlebars.Compiler.AST
{
  internal class CommentLiteral : ASTElementBase
  {
    internal readonly string Value;
    internal readonly CommentType Type;
    internal CommentLiteral(CommentType type, string value, int line, int column) : base(line, column)
    {
      Type = type;
      Value = value;
    }

    internal override void Accept(IASTVisitor visitor)
    {
      visitor.Visit(this);
    }
  }

  internal enum CommentType { Single, Multi }
}
