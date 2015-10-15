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
    public List<HandlebarsException> Errors { get; private set; } = new List<HandlebarsException>();
    public RoslynIntrospector Introspector { get; set; }
    public Stack<List<StatementSyntax>> resultStack { get; private set; } = new Stack<List<StatementSyntax>>();
    internal Stack<Context> ContextStack { get; set; } = new Stack<Context>();
    internal HandlebarsTemplate Template { get; private set; }

    public CompilationState(RoslynIntrospector introspector, HandlebarsTemplate template)
    {
      Introspector = introspector;
      Template = template; 
      INamedTypeSymbol modelSymbol = Introspector.GetTypeSymbol(Template.Model.ToString());
      if (modelSymbol == null)
        Errors.Add(new HandlebarsTypeError($"Could not find Type in ModelToken '{Template.Model.ToString()}'!", 1, 1));
      ContextStack.Push(new Context("viewModel", modelSymbol));
      resultStack.Push(new List<StatementSyntax>());
    }

    public void AddTypeError(string message)
    {
      Errors.Add(new HandlebarsTypeError(message, line, column));
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

    internal void SetCursor(ASTElementBase element)
    {
      line = element.Line;
      column = element.Column;
    }


  }
}
