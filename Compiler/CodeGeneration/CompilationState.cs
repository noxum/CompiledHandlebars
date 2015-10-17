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
                SyntaxHelper.IsTruthyMethodObject()
              )
          )
      );
      return compiledHbs;
    }

    internal void PromiseTruthyCheck(Context contextToCheck)
    {
      TruthyStack.Push(contextToCheck);
    }

    internal void DoTruthyCheck(List<StatementSyntax> ifBlock, List<StatementSyntax> elseBlock = null, AST.IfType ifType = IfType.If)
    {
      var contextToCheck = TruthyStack.Pop();
      IfStatementSyntax ifStatement;
      if (TruthyStack.Any())
        ifStatement = SyntaxHelper.IfIsTruthy(TruthyStack.Peek(), contextToCheck, ifType);
      else
        ifStatement = SyntaxHelper.IfIsTruthy(null, contextToCheck, ifType);
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
