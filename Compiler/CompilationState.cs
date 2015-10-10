using CompiledHandlebars.Compiler.AST;
using CompiledHandlebars.Compiler.CodeGeneration;
using CompiledHandlebars.Compiler.Introspection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler
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

    public void StartBlock()
    {

    }

    public void EndBlock()
    {

    }

    public CompilationUnitSyntax GetCompilationUnit()
    {
      var ws = new AdhocWorkspace();
      var compiledHbs = SyntaxFactory.CompilationUnit()
      .AddUsings(
        SyntaxHelper.UsingDirectives
      )
      .AddMembers(
        SyntaxHelper.HandlebarsNamespace(Template.Namespace)
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
      return compiledHbs;
    }

    internal void SetCursor(ASTElementBase element)
    {
      line = element.Line;
      column = element.Column;
    }


  }
}
