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
    private List<string> usings { get; set; } = new List<string>() { "System", "System.Linq", "System.Net", "System.Text", "System.Collections.Generic" };
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

    internal CompilationState(RoslynIntrospector introspector, HandlebarsTemplate template)
    {
      Introspector = introspector;
      Template = template; 
      if (!(template is StaticHandlebarsTemplate))
      {
        INamedTypeSymbol modelSymbol = Introspector.GetTypeSymbol(Template.Model.ToString());
        if (modelSymbol == null)
          Errors.Add(new HandlebarsTypeError($"Could not find Type in ModelToken '{Template.Model.ToString()}'!", HandlebarsTypeErrorKind.UnknownViewModel, 1, 1));
        ContextStack.Push(new Context("viewModel", modelSymbol));
      }
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


    internal CompilationUnitSyntax GetCompilationUnitStaticTemplate()
    {
      if (resultStack.Count == 1)
      {
        var additionalMemberSyntax = GetAdditionalMembers();
        additionalMemberSyntax.Add(SyntaxHelper.CompiledHandlebarsTemplateAttributeClass());
        var usingsSyntax = GetUsingDirectives();
        return SyntaxFactory.CompilationUnit()
        .AddUsings(
          usingsSyntax.ToArray()
        )
        .AddMembers(
          SyntaxHelper.HandlebarsNamespace(Template.Namespace)
            .AddMembers(
              SyntaxHelper.CompiledHandlebarsClassDeclaration(Template.Name, StringConstants.TEMPLATEATTRIBUTE)
                .AddMembers(
                  SyntaxHelper.RenderWithoutParameter()
                    .AddBodyStatements(
                      resultStack.Pop().ToArray()
                    )
                ).AddMembers(
                  additionalMemberSyntax.ToArray()
                )
            )
        );
      }
      return SyntaxFactory.CompilationUnit();
    }

    internal CompilationUnitSyntax GetCompilationUnitHandlebarsLayout()
    {
      //Two Lists of StatementSyntax for the body of two render methods
      if (resultStack.Count == 2)
      {
        var additionalMemberSyntax = GetAdditionalMembers();
        additionalMemberSyntax.Add(SyntaxHelper.CompiledHandlebarsLayoutAttributeClass());
        var usingsSyntax = GetUsingDirectives();
        return SyntaxFactory.CompilationUnit()
        .AddUsings(
          usingsSyntax.ToArray()
        )
        .AddMembers(
          SyntaxHelper.HandlebarsNamespace(Template.Namespace)
            .AddMembers(
              SyntaxHelper.CompiledHandlebarsClassDeclaration(Template.Name, StringConstants.LAYOUTATTRIBUTE)
                .AddMembers(
                  SyntaxHelper.RenderWithParameter(Template.Model.ToString(), "PostRender")
                    .AddBodyStatements(
                      resultStack.Pop().ToArray()
                    ),
                  SyntaxHelper.RenderWithParameter(Template.Model.ToString(), "PreRender")
                    .AddBodyStatements(
                      resultStack.Pop().ToArray()
                    )
                ).AddMembers(
                  additionalMemberSyntax.ToArray()
                )
            )
        );
      }
      return SyntaxFactory.CompilationUnit();
    }

    //TODO: Too similar to GetCompilationUnitHandlebarsLayout. Clean up
    internal CompilationUnitSyntax GetCompilationUnitHandlebarsTemplate()
    {
      //One List of StatementSyntax for the body of one render method
      if (resultStack.Count == 1)
      {
        var additionalMemberSyntax = GetAdditionalMembers();
        additionalMemberSyntax.Add(SyntaxHelper.CompiledHandlebarsTemplateAttributeClass());
        var usingsSyntax = GetUsingDirectives();
        return SyntaxFactory.CompilationUnit()
        .AddUsings(
          usingsSyntax.ToArray()
        )
        .AddMembers(
          SyntaxHelper.HandlebarsNamespace(Template.Namespace)
            .AddMembers(
              SyntaxHelper.CompiledHandlebarsClassDeclaration(Template.Name, StringConstants.TEMPLATEATTRIBUTE)
                .AddMembers(
                  SyntaxHelper.RenderWithParameter(Template.Model.ToString())
                    .AddBodyStatements(
                      resultStack.Pop().ToArray()
                    )
                ).AddMembers(
                  additionalMemberSyntax.ToArray()
                )
            )
        );        
      }
      return SyntaxFactory.CompilationUnit();      
    }

    private List<MemberDeclarationSyntax> GetAdditionalMembers()
    {
      var result = new List<MemberDeclarationSyntax>();
      if (!Introspector.RuntimeUtilsReferenced())
      {
        result.Add(SyntaxHelper.IsTruthyMethodBool());
        result.Add(SyntaxHelper.IsTruthyMethodString());
        result.Add(SyntaxHelper.IsTruthyMethodObject());
        result.Add(SyntaxHelper.IsTruthyMethodIEnumerableT());
      }
      return result;
    }

    private List<UsingDirectiveSyntax> GetUsingDirectives()
    {
      var result = new List<UsingDirectiveSyntax>();
      result.AddRange(usings.Select(x => SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(x))));
      if (Introspector.RuntimeUtilsReferenced())
      {
        result.Add(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("CompiledHandlebars.RuntimeUtils")));
        result.Add(SyntaxHelper.UsingStatic("CompiledHandlebars.RuntimeUtils.RenderHelper"));
      }
      return result;
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
    {//Items -> ItemsTitle
      var resultList = new List<string>();      
      var lastCheckedElements = lastCheckedContext?.FullPath.Split('.') ?? new string[0];
      var pathToCheckElements = contextToCheck.FullPath.Split('.');        
      List<string> relevantElements = new List<string>();
      List<string> prefixElements = new List<string>();
      if (lastCheckedElements.Any())
      {
        int i = 0;
        for (i = 0; i < lastCheckedElements.Length; i++)
        {
          if (lastCheckedElements[i].Equals(pathToCheckElements[i]))
            prefixElements.Add(lastCheckedElements[i]);
          else
            break;
        }
        relevantElements.AddRange(pathToCheckElements.Skip(i));
      }
      else
        relevantElements = pathToCheckElements.ToList();
      string prefix = prefixElements.Any() ? string.Join(".", prefixElements) + ".": string.Empty;
      for (int i = 1; i <= relevantElements.Count; i++)
      {//then join them back together with the prefix        
        resultList.Add(string.Concat(prefix, string.Join(".", relevantElements.Take(i).ToArray())));
      }
      return resultList;
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
