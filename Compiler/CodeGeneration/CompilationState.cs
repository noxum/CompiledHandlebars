using CompiledHandlebars.Compiler.AST;
using CompiledHandlebars.Compiler.Introspection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler.CodeGeneration
{
  public class CompilationState
  {
    private int line { get; set; }
    private int column { get; set; }
    private Stack<List<StatementSyntax>> resultStack { get; set; } = new Stack<List<StatementSyntax>>();
    private List<string> usings { get; set; } = new List<string>() { "System", "System.Linq", "System.Net", "System.Text" };
    internal int LoopLevel { get; set; } = 0;
    internal RoslynIntrospector Introspector { get; set; }
    internal HandlebarsTemplate Template { get; private set; }
    internal List<HandlebarsException> Errors { get; private set; } = new List<HandlebarsException>();
    internal Stack<Context> ContextStack { get; set; } = new Stack<Context>();   
     
    /// <summary>
    /// Contains the status about already checked variables.
    /// Needs to be seperated from the context stack, as truthyness check does not always change context (e.g. #if)
    /// </summary>
    internal Stack<Context> TruthyStack { get; set; } = new Stack<Context>();   

    public CompilationState(RoslynIntrospector introspector, HandlebarsTemplate template)
    {
      Introspector = introspector;
      Template = template; 
      INamedTypeSymbol modelSymbol = Introspector.GetTypeSymbol(Template.Model.ToString());
      if (modelSymbol == null)
        Errors.Add(new HandlebarsTypeError($"Could not find Type in ModelToken '{Template.Model.ToString()}'!", HandlebarsTypeErrorKind.UnknownViewModel, 1, 1));
      ContextStack.Push(new Context("viewModel", modelSymbol));
      resultStack.Push(new List<StatementSyntax>());
    }

    internal void AddTypeError(string message, HandlebarsTypeErrorKind kind)
    {
      Errors.Add(new HandlebarsTypeError(message, kind, line, column));
    }

    internal void AddTypeError(HandlebarsTypeError error)
    {
      Errors.Add(error);
    }

    internal void PushStatement(StatementSyntax statement)
    {
      resultStack.Peek().Add(statement);
    }

    internal void PushNewBlock()
    {
      resultStack.Push(new List<StatementSyntax>());
    }

    internal List<StatementSyntax> PopBlock()
    {
      return resultStack.Pop();
    }
    
    public void AddComment(string comment)
    {
      PushStatement(SyntaxHelper.EmptyStatementWithComment(comment));
    }

    internal void SetFirstVariable()
    {
      PushStatement(SyntaxHelper.AssignFalse($"first{LoopLevel}"));
    }

    internal void SetLastVariable(string loopedVariable)
    {
      PushStatement(SyntaxHelper.AssignValueEqualsValue($"last{LoopLevel}", $"index{(LoopLevel)}", $"({loopedVariable}.Count()-1)"));
    }

    internal void IncrementIndexVariable()
    {
      PushStatement(SyntaxHelper.IncrementVariable($"index{LoopLevel}"));
    }

    internal void RegisterUsing(string nameSpace)
    {
      if (!Template.Namespace.Equals(nameSpace) && !usings.Contains(nameSpace))
        usings.Add(nameSpace);
    }

    internal CompilationUnitSyntax GetCompilationUnit(string nameSpaceComment)
    {
      var ws = new AdhocWorkspace();      
      if (resultStack.Any())
      {
        if (Introspector.RuntimeUtilsReferenced())
        {
          return SyntaxFactory.CompilationUnit()
          .AddUsings(
            SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("CompiledHandlebars.RuntimeUtils")),
            SyntaxHelper.UsingStatic("CompiledHandlebars.RuntimeUtils.RenderHelper")
          )
          .AddUsings(
            usings.Select(x => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(x))).ToArray()
          )
          .AddMembers(
            SyntaxHelper.HandlebarsNamespace(Template.Namespace, nameSpaceComment)
              .AddMembers(
                SyntaxHelper.CompiledHandlebarsClassDeclaration(Template.Name)
                  .AddMembers(
                    SyntaxHelper.RenderWithParameter(Template.Model.ToString())
                      .AddBodyStatements(
                        resultStack.Pop().ToArray()
                      )
                  )
              )
          );
        }
        else
        {
          return SyntaxFactory.CompilationUnit()
          .AddUsings(
            usings.Select(x => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(x))).ToArray()
          )
          .AddMembers(
            SyntaxHelper.HandlebarsNamespace(Template.Namespace, nameSpaceComment)
              .AddMembers(
                SyntaxHelper.CompiledHandlebarsClassDeclaration(Template.Name)
                  .AddMembers(
                    SyntaxHelper.RenderWithParameter(Template.Model.ToString())
                      .AddBodyStatements(
                        resultStack.Pop().ToArray()
                      ),
                    SyntaxHelper.IsTruthyMethodBool(),
                    SyntaxHelper.IsTruthyMethodString(),
                    SyntaxHelper.IsTruthyMethodObject(),
                    SyntaxHelper.CompiledHandlebarsTemplateAttributeClass()
                  )
              )
          );
        }
      }
      return SyntaxFactory.CompilationUnit();      
    }

    internal void PromiseTruthyCheck(Context contextToCheck, IfType ifType = IfType.If)
    {
      contextToCheck.Truthy = ifType == IfType.If;
      TruthyStack.Push(contextToCheck);
    }

    internal void DoTruthyCheck(List<StatementSyntax> ifBlock, List<StatementSyntax> elseBlock = null, AST.IfType ifType = IfType.If)
    {
      var contextToCheck = TruthyStack.Pop();
      IfStatementSyntax ifStatement;
      if (TruthyStack.Any())
        ifStatement = SyntaxHelper.IfIsTruthy(GetQueryElements(TruthyStack.Peek(), contextToCheck), ifType);
      else
        ifStatement = SyntaxHelper.IfIsTruthy(GetQueryElements(null, contextToCheck), ifType);
      if (ifStatement == null)
      {
        if (elseBlock != null)
          AddTypeError("Unreachable 'else' Block", HandlebarsTypeErrorKind.UnreachableCode);
        resultStack.Peek().AddRange(ifBlock);
      } else
      {
        if (elseBlock != null)
        {
          resultStack.Peek().Add(
            ifStatement.WithStatement(SyntaxFactory.Block(ifBlock)).
            WithElse(SyntaxFactory.ElseClause(SyntaxFactory.Block(elseBlock)))
          );
        } else
        {
          resultStack.Peek().Add(
            ifStatement.WithStatement(SyntaxFactory.Block(ifBlock)));
        }
      }
    }

    /// <summary>
    /// Returns a list of strings of the elements of a context which needs to be checked.
    /// I.e. if the path in contextToCheck is "viewModel.Parent.Child" and the path in lastCheckedContext is 
    /// "viewModel" it will return { "viewModel.Parent", "viewModel.Parent.Child" }
    /// Also unreachable code is detected
    /// </summary>
    /// <param name="lastCheckedContext"></param>
    /// <param name="contextToCheck"></param>
    /// <returns></returns>
    private List<string> GetQueryElements(Context lastCheckedContext, Context contextToCheck)
    {
      var argumentList = new List<string>();
      var pathToCheck = contextToCheck.FullPath;
      if (lastCheckedContext != null
          && contextToCheck.FullPath.StartsWith(lastCheckedContext.FullPath)
          && lastCheckedContext.FullPath.Contains("."))
      {//The context to check is directly depended from the context checked before
        if (lastCheckedContext.Truthy != contextToCheck.Truthy)//Unreachable Code
          AddTypeError("Unreachable code detected", HandlebarsTypeErrorKind.UnreachableCode); 
        if (lastCheckedContext.FullPath.Equals(contextToCheck.FullPath))
          return null;//Context has already been checked. Nothing to check
        //Get the unchecked subpath
        pathToCheck = contextToCheck.FullPath.Substring(lastCheckedContext.FullPath.Length + 1);
        //Split it into elements
        var elements = pathToCheck.Split('.');
        for(int i = 1; i<=elements.Length;i++)
        {//then join them back together with the prefix
          argumentList.Add(string.Join(".", lastCheckedContext.FullPath, string.Join(".", elements.Take(i).ToArray())));
        }
      } else
      {//The context to check is independed from the context checked before
        var elements = pathToCheck.Split('.');
        for (int i = 1; i <= elements.Length; i++)
        {
          argumentList.Add(string.Join(".", elements.Take(i).ToArray()));
        }
      }
      return argumentList;
    }

    internal Context BuildLoopContext(ITypeSymbol symbol)
    {
      return new Context($"loopItem{LoopLevel}", symbol);
    }

    internal void SetCursor(ASTElementBase element)
    {
      line = element.Line;
      column = element.Column;
    }


  }
}
