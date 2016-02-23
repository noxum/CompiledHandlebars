using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompiledHandlebars.Compiler.AST;
using CompiledHandlebars.Compiler.Introspection;
using CompiledHandlebars.Compiler.CodeGeneration;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using CompiledHandlebars.Compiler.AST.Expressions;
using Microsoft.CodeAnalysis;

namespace CompiledHandlebars.Compiler.Visitors
{
  internal class CodeGenerationVisitor : IASTVisitor
  {
    private CompilationState state { get; set; }
    private CompilationUnitSyntax resultingCompilationUnit { get; set; } 


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
    

    public CompilationUnitSyntax GenerateCode()
    {
      try
      {
        state.Template.Accept(this);   
        //If resultingCompilationUnit is null, compilation failed and errors where written -> return Empty CompilationUnit
        return resultingCompilationUnit?? SyntaxFactory.CompilationUnit();
      } catch(HandlebarsTypeError e)
      {
        state.AddTypeError(e);
      } catch(Exception e)
      {
        state.AddTypeError($"Compilation failed: {e.Message}", HandlebarsTypeErrorKind.CompilationFailed);
      }
      return SyntaxFactory.CompilationUnit();
    }

    public void Visit(MarkupLiteral astLeaf)
    {
      state.SetCursor(astLeaf);
      if (!string.IsNullOrEmpty(astLeaf.Value))
      {//Do not append empty strings (possible through whitespace control)
        state.PushStatement(SyntaxHelper.AppendStringLiteral(astLeaf.Value));
      }
    }

    public void Visit(YieldStatement astLeaf)
    {
      state.SetCursor(astLeaf);
      Context yieldContext;
      if (astLeaf.Expr.TryEvaluate(state, out yieldContext))
      {
        state.PushStatement(SyntaxHelper.AppendMember(
          yieldContext.FullPath, 
          yieldContext.Symbol.IsString(), 
          astLeaf.Type == TokenType.Encoded)
        );
      } else
      {
        //Unknown Member could also be a HelperCall with implied this as Parameter
        if (astLeaf.Expr is MemberExpression)
          astLeaf.TransformToHelperCall().Accept(this);
      }
    }


    public void VisitEnter(WithBlock astNode)
    {
      state.SetCursor(astNode);
      state.PushNewBlock();
      //Enter new Context and promise to check its truthyness
      Context context;
      if (astNode.Expr.TryEvaluate(state, out context))
      {
        state.PromiseTruthyCheck(context);
        state.ContextStack.Push(context);
      }
    }

    public void VisitLeave(WithBlock astNode)
    {
      //Leave Context
      state.ContextStack.Pop();
      var latestBlock = state.PopBlock();
      if (astNode.HasElseBlock)
        state.DoTruthyCheck(state.PopBlock(), latestBlock, IfType.If);
      else
        state.DoTruthyCheck(latestBlock, ifType: IfType.If);
    }

    public void VisitElse(WithBlock astNode)
    {
      //Leave Context
      state.ContextStack.Pop();
      var truthyContext = state.TruthyStack.Pop();
      truthyContext.Truthy = !truthyContext.Truthy;
      state.TruthyStack.Push(truthyContext);
      state.PushNewBlock();
    }

    public void VisitEnter(IfBlock astNode)
    {
      state.SetCursor(astNode);
      Context context;
      if (astNode.Expr.TryEvaluate(state, out context))
      {
        state.PushNewBlock();
        state.PromiseTruthyCheck(context, astNode.QueryType);
      }
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
      //Check if we are already in a compiletime loop. 
      if (astNode.Type!=EachBlock.LoopType.CompileTime)
      {//Not in compiletime loop. procede as usual
        Context loopedVariable;
        if (astNode.Member.TryEvaluate(state, out loopedVariable))
        { 
          state.PromiseTruthyCheck(loopedVariable);
          state.PushNewBlock();
          Context loopContext = astNode.Member.EvaluateLoop(state);
          if (loopContext!=null)
          {//The loopVariable implements IEnumerable -> runtimeloop over its contents
            astNode.Type = EachBlock.LoopType.RunTime;
            state.ContextStack.Push(astNode.Member.EvaluateLoop(state));   
            state.LoopLevel++;        
            if (astNode.Flags.HasFlag(EachBlock.ForLoopFlags.Last))
              state.SetLastVariable(loopedVariable.FullPath);
          }
          else
          {//The loopVariable is an object. Compiletimeloop over its Members
            astNode.Type = EachBlock.LoopType.CompileTime;
            //Push the CompileTimeLoopContext
            var ctLoopContext = new CompileTimeLoopContext(loopedVariable.FullPath, loopedVariable.Symbol);
            ctLoopContext.MoveNext();
            state.ContextStack.Push(ctLoopContext);
          }
        }
      }
    }

    public void VisitLeave(EachBlock astNode)
    {      
      state.SetCursor(astNode);
      if (astNode.Type==EachBlock.LoopType.RunTime)
      {
        if (astNode.Flags.HasFlag(EachBlock.ForLoopFlags.Index) || astNode.Flags.HasFlag(EachBlock.ForLoopFlags.Last))
          state.IncrementIndexVariable();
        if (astNode.Flags.HasFlag(EachBlock.ForLoopFlags.First))
          state.SetFirstVariable();
        //Leave loop context
        state.ContextStack.Pop();
        state.LoopLevel--;
        var prepareLoopStatements = SyntaxHelper.PrepareForLoop(astNode.Flags, state.LoopLevel + 1);
        Context context;
        if (astNode.Member.TryEvaluate(state, out context))
        {
          prepareLoopStatements.Add(SyntaxHelper.ForLoop(astNode.Member.EvaluateLoop(state).FullPath, context.FullPath, state.PopBlock()));
        }
        state.DoTruthyCheck(
            prepareLoopStatements
        );
      } else if (astNode.Type==EachBlock.LoopType.CompileTime)
      {
        //Check if loop is completed
        var ctLoopContext = state.ContextStack.Peek() as CompileTimeLoopContext;
        if (ctLoopContext.MoveNext())
        {//Next Iteration
          astNode.Accept(this);
        }
        else
        {//Pop ctLoopContext
          state.ContextStack.Pop();         
          state.DoTruthyCheck(state.PopBlock());
        }
      }
    }

    public void Visit(PartialCall astLeaf)
    {
      state.SetCursor(astLeaf);
      Context argumentContext;
      if (astLeaf.Expr.TryEvaluate(state, out argumentContext))
      {
        if (astLeaf.TemplateName.ToString().Equals(state.Template.Name, StringComparison.OrdinalIgnoreCase))
        {//Self referencing Template
          state.PushStatement(SyntaxHelper.SelfReferencingPartialCall(argumentContext.FullPath));
        }
        else
        {
          var partial = state.Introspector.GetPartialHbsTemplate(astLeaf.TemplateName.ToString());
          if (partial == null)
          {        
            state.AddTypeError($"Could not find partial '{astLeaf.TemplateName}'", HandlebarsTypeErrorKind.UnknownPartial);
            return;
          }
          state.RegisterUsing(partial.ContainingNamespace.ToDisplayString());
          state.PushStatement(
            SyntaxHelper.HbsTemplateCall(
              partial.Name,
              argumentContext.FullPath));        
        }
      }
    }

    public void Visit(HelperCall astLeaf)
    {
      state.SetCursor(astLeaf);
      var paramContextList = new List<Context>();
      foreach(var param in astLeaf.Parameters)
      {
        Context paramContext;
        if (param.TryEvaluate(state, out paramContext))
          paramContextList.Add(paramContext);
      }            
      var helperMethod = state.Introspector.GetHelperMethod(astLeaf.FunctionName, paramContextList.Select(x => x.Symbol).ToList());
      if (helperMethod != null)
      {
        state.RegisterUsing(helperMethod.ContainingNamespace.ToDisplayString());
        state.PushStatement(
          SyntaxHelper.AppendFuntionCallResult(
            functionName: string.Concat(helperMethod.ContainingType.Name,".",helperMethod.Name),
            parameters: paramContextList.Select(x => x.FullPath).ToList(),
            returnTypeIsString: helperMethod.ReturnType.IsString(),
            encoded: astLeaf.Type == TokenType.Encoded));       
      } 
      else
      {//HelperMethod not found
        state.AddTypeError($"Could not find Helper Method '{astLeaf.FunctionName}'", HandlebarsTypeErrorKind.UnknownHelper);
        return;
      }
    }


    public void VisitEnter(HandlebarsTemplate template)
    {
      state.PushStatement(SyntaxHelper.DeclareAndCreateStringBuilder);
    }


    public void VisitLeave(HandlebarsTemplate template)
    {
      state.PushStatement(SyntaxHelper.ReturnSBToString);
      resultingCompilationUnit = state.GetCompilationUnitHandlebarsTemplate();
    }

    public void VisitEnter(LayoutedHandlebarsTemplate layoutedTemplate)
    {
      VisitEnter(layoutedTemplate as HandlebarsTemplate);
      var layout = state.Introspector.GetLayoutHbsTemplate(layoutedTemplate.LayoutName);
      if (layout == null)
      {
        state.AddTypeError($"Could not find layout '{layoutedTemplate.LayoutName}'", HandlebarsTypeErrorKind.UnknownLayout);
        return;
      }
      state.RegisterUsing(layout.ContainingNamespace.ToDisplayString());
      state.PushStatement(
        SyntaxHelper.HbsTemplateCall(
          layout.Name,
          //Correct here as the ContextStack contains only the root context at the beginning
          state.ContextStack.Peek().FullPath, 
          methodName: "PreRender"));              
    }

    public void VisitLeave(LayoutedHandlebarsTemplate layoutedTemplate)
    {
      var layout = state.Introspector.GetLayoutHbsTemplate(layoutedTemplate.LayoutName);
      if (layout == null)
      {
        state.AddTypeError($"Could not find layout '{layoutedTemplate.LayoutName}'", HandlebarsTypeErrorKind.UnknownLayout);
        return;
      }
      state.PushStatement(
        SyntaxHelper.HbsTemplateCall(
          layout.Name,
          //Correct here as the ContextStack contains only the root context at the end
          state.ContextStack.Peek().FullPath,
          methodName: "PostRender"));
      VisitLeave(layoutedTemplate as HandlebarsTemplate);
    }


    public void VisitRenderBody(HandlebarsLayout layout)
    {
      state.PushStatement(SyntaxHelper.ReturnSBToString);
      state.PushNewBlock();
      state.PushStatement(SyntaxHelper.DeclareAndCreateStringBuilder);
    }

    public void VisitLeave(HandlebarsLayout layout)
    {
      state.PushStatement(SyntaxHelper.ReturnSBToString);
      resultingCompilationUnit = state.GetCompilationUnitHandlebarsLayout();
    }

    public void VisitLeave(StaticHandlebarsTemplate staticTemplate)
    {
      state.PushStatement(SyntaxHelper.ReturnSBToString);
      resultingCompilationUnit = state.GetCompilationUnitStaticTemplate();
    }
  }
}
