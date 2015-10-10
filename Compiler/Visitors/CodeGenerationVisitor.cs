using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.AST;

namespace CompiledHandlebars.Compiler.Visitors
{
  internal class CodeGenerationVisitor : IASTVisitor
  {
    public void Visit(MarkupLiteral astLeaf)
    {
      throw new NotImplementedException();
    }

    public void Visit(YieldStatement astLeaf)
    {
      throw new NotImplementedException();
    }

    public void VisitEnter(HandlebarsTemplate template)
    {
      throw new NotImplementedException();
    }

    public void VisitLeave(HandlebarsTemplate template)
    {
      throw new NotImplementedException();
    }

    internal string GenerateCode(HandlebarsTemplate template)
    {
      return "Hello World";
    }

  }
}
