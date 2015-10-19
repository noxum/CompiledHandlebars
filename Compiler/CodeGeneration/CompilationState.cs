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
    public string ResultingCode { get; set; }
    public int loopLevel { get; set; } = 0;
    public List<HandlebarsException> Errors { get; private set; } = new List<HandlebarsException>();
    public RoslynIntrospector Introspector { get; set; }
    public Stack<List<StatementSyntax>> resultStack { get; private set; } = new Stack<List<StatementSyntax>>();
    internal Stack<Context> ContextStack { get; set; } = new Stack<Context>();
    /// <summary>
    /// Contains the status about already checked variables.
    /// Needs to be seperated from the context stack, as truthyness check does not always change context (e.g. #if)
    /// </summary>
    internal Stack<Context> TruthyStack { get; set; } = new Stack<Context>();
    internal HandlebarsTemplate Template { get; private set; }

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

    public void AddTypeError(string message, HandlebarsTypeErrorKind kind)
    {
      Errors.Add(new HandlebarsTypeError(message, kind, line, column));
    }

    public void AddTypeError(HandlebarsTypeError error)
    {
      Errors.Add(error);
    }

    public void PushStatement(StatementSyntax statement)
    {
      resultStack.Peek().Add(statement);
    }

    public void PushNewBlock()
    {
      resultStack.Push(new List<StatementSyntax>());
    }

    public List<StatementSyntax> PopBlock()
    {
      return resultStack.Pop();
    }
    
    public void AddComment(string comment)
    {
      var list = resultStack.Peek();
      //Very beautiful...
      //Maybe stack<stack<StatementSyntax>>
      list[list.Count-1] = SyntaxHelper.AddCommentToStatement(list[list.Count - 1], comment);
    }

    public CompilationUnitSyntax GetCompilationUnit(string nameSpaceComment)
    {
      var ws = new AdhocWorkspace();
      var compiledHbs = SyntaxFactory.CompilationUnit()
      .AddUsings(
        SyntaxHelper.UsingDirectives
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
      return compiledHbs;
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

    internal Context BuildLoopContext(ISymbol symbol)
    {
      return new Context($"loopItem{loopLevel}", symbol);
    }

    internal void SetCursor(ASTElementBase element)
    {
      line = element.Line;
      column = element.Column;
    }


  }
}
