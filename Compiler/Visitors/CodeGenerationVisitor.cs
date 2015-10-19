﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.AST;
using CompiledHandlebars.Compiler.Introspection;
using CompiledHandlebars.Compiler.CodeGeneration;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CompiledHandlebars.Compiler.Visitors
{
  internal class CodeGenerationVisitor : IASTVisitor
  {
    private CompilationState state { get; set; }

    public List<HandlebarsException> ErrorList
    {
      get
      {
        return state.Errors;
      }
    }
    public CodeGenerationVisitor(RoslynIntrospector introspector, HandlebarsTemplate template)
    {
      state = new CompilationState(introspector, template);
      state.Introspector = introspector;
    }
    public void GenerateCode()
    {
      try
      {
        state.Template.Accept(this);
      } catch(HandlebarsTypeError e)
      {
        state.AddTypeError(e);
      } catch(Exception e)
      {
        state.AddTypeError($"Compilation failed: {e.Message}", HandlebarsTypeErrorKind.CompilationFailed);
      }
    }
    public CompilationUnitSyntax CompilationUnit(string templateComment)
    {
      return state.GetCompilationUnit(templateComment);
    }

    public void Visit(MarkupLiteral astLeaf)
    {
      state.SetCursor(astLeaf);
      if (!string.IsNullOrEmpty(astLeaf._Value))
      {//Do not append empty strings (possible through whitespace control)
        state.PushStatement(SyntaxHelper.AppendStringLiteral(astLeaf._Value));
      }
    }

    public void Visit(YieldStatement astLeaf)
    {
      state.SetCursor(astLeaf);
      if (astLeaf._type == TokenType.Encoded)
        state.PushStatement(SyntaxHelper.AppendMemberEncoded(astLeaf.Member.Evaluate(state).FullPath));
      else
        state.PushStatement(SyntaxHelper.AppendMember(astLeaf.Member.Evaluate(state).FullPath));
    }

    public void VisitEnter(HandlebarsTemplate template)
    {
      state.PushStatement(SyntaxHelper.DeclareAndCreateStringBuilder);
    }

    public void VisitLeave(HandlebarsTemplate template)
    {
      state.PushStatement(SyntaxHelper.ReturnSBToString);
    }

    public void VisitEnter(WithBlock astNode)
    {
      state.SetCursor(astNode);
      state.PushNewBlock();
      //Enter new Context and promise to check its truthyness
      state.PromiseTruthyCheck(astNode.Member.Evaluate(state));
      state.ContextStack.Push(astNode.Member.Evaluate(state));
    }

    public void VisitLeave(WithBlock astNode)
    {
      //Leave Context
      state.ContextStack.Pop();
      state.DoTruthyCheck(state.PopBlock());
    }

    public void VisitEnter(IfBlock astNode)
    {
      state.SetCursor(astNode);
      state.PushNewBlock();
      state.PromiseTruthyCheck(astNode.Member.Evaluate(state), astNode.QueryType);
    }

    public void VisitLeave(IfBlock astNode)
    {
      var latestBlock = state.PopBlock();
      if (astNode.HasElseBlock)
        state.DoTruthyCheck(state.PopBlock(), latestBlock, astNode.QueryType);
      else
        state.DoTruthyCheck(latestBlock, ifType: astNode.QueryType);          
    }

    public void VisitElse(IfBlock astNode)
    {
      var truthyContext = state.TruthyStack.Pop();
      truthyContext.Truthy = !truthyContext.Truthy;
      state.TruthyStack.Push(truthyContext);
      state.PushNewBlock();
    }

    public void Visit(CommentLiteral astLeaf)
    {
      state.AddComment(astLeaf.Value);
    }

    public void VisitEnter(EachBlock astNode)
    {
      state.SetCursor(astNode);
      state.PromiseTruthyCheck(astNode.Member.Evaluate(state));
      state.ContextStack.Push(astNode.Member.EvaluateLoop(state));
      state.PushNewBlock();
      state.loopLevel++;
    }

    public void VisitLeave(EachBlock astNode)
    {
      //Leave loop context
      state.ContextStack.Pop();
      state.loopLevel--;
      state.DoTruthyCheck(
        new List<StatementSyntax>()
        {
          SyntaxHelper.ForLoop(astNode.Member.EvaluateLoop(state).FullPath, astNode.Member.Evaluate(state).FullPath, state.PopBlock())
        }                        
      );
    }

    public void Visit(PartialCall astLeaf)
    {
      state.SetCursor(astLeaf);
      string memberName = astLeaf._HasMember ?
                            astLeaf._Member.Evaluate(state).FullPath :
                            state.ContextStack.Peek().FullPath;
      if (astLeaf._TemplateName.Equals(state.Template.Name))
      {//Self referencing Template
        state.PushStatement(SyntaxHelper.SelfReferencingPartialCall(memberName));
      }
      else
      {
        var partial = state.Introspector.GetPartialHbsTemplate(astLeaf._TemplateName);
        if (partial == null)
        {        
          state.AddTypeError($"Could not find partial '{astLeaf._TemplateName}'", HandlebarsTypeErrorKind.UnknownPartial);
          return;
        }        
        state.PushStatement(
          SyntaxHelper.PartialTemplateCall(
            partial.Name, 
            memberName));        
      }
    }
  }
}
