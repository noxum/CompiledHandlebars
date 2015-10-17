using CompiledHandlebars.Compiler.Introspection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CompiledHandlebars.Compiler.CodeGeneration
{
  /// <summary>
  /// Reocurring neverchanging SyntaxNodes
  /// </summary>
  internal static class SyntaxHelper
  {
    /// <summary>
    /// Yields code that declares and creates a StringBuilder:
    /// var sb = new StringBuilder();
    /// </summary>
    internal static LocalDeclarationStatementSyntax DeclareAndCreateStringBuilder =
      SF.LocalDeclarationStatement(
        SF.VariableDeclaration(
            SF.ParseTypeName("var"), //Only way to get var as it was only introduced in C#4.0 and might break code before that. Nevertheless: THEFUCK?!
            new SeparatedSyntaxList<VariableDeclaratorSyntax>().Add(
              SF.VariableDeclarator(SF.Identifier("sb"), default(BracketedArgumentListSyntax),
              SF.EqualsValueClause(
                SF.ObjectCreationExpression(
                  SF.ParseTypeName("StringBuilder"),
                  SF.ArgumentList(),
                  default(InitializerExpressionSyntax)
                )
              )
            )
          )
        )
      );


    /// <summary>
    /// Yields a return statement for the Stringbuilder.ToString() Method:
    /// return sb.ToString();
    /// </summary>
    internal static ReturnStatementSyntax ReturnSBToString =
      SF.ReturnStatement(
        SF.Token(SyntaxKind.ReturnKeyword),
        SF.InvocationExpression(SF.ParseExpression("sb.ToString")),
        SF.Token(SyntaxKind.SemicolonToken)
      );


    /// <summary>
    /// Yields using Directives:
    /// using System.Text;
    /// </summary>
    internal static UsingDirectiveSyntax[] UsingDirectives =
      new UsingDirectiveSyntax[]
      {
      SF.UsingDirective(SF.ParseName("System.Text")),
      SF.UsingDirective(SF.ParseName("System.Net"))
      };


    /// <summary>
    /// Yields a NamespaceDeclaration:
    /// namespace CompiledHandlebars{}
    /// </summary>
    internal static NamespaceDeclarationSyntax HandlebarsNamespace(string nameSpace, string comment)
    {
      return SF.NamespaceDeclaration(SF.ParseName(nameSpace))
              .WithLeadingTrivia(
                SF.SyntaxTrivia(SyntaxKind.MultiLineCommentTrivia, string.Concat("/*",comment,"*/")));
    }


    /// <summary>
    /// Yields the CompiledHandlebars Class Declaration ClassDeclaration
    /// public static class CompiledHandlebarsTemplate<TViewModel> {}
    /// </summary>
    internal static ClassDeclarationSyntax CompiledHandlebarsClassDeclaration(string templateName)
    {
      return
        SF.ClassDeclaration(
          new SyntaxList<AttributeListSyntax>(),
          SF.TokenList(
            SF.Token(SyntaxKind.PublicKeyword),
            SF.Token(SyntaxKind.StaticKeyword)),
          SF.Identifier(templateName),
          default(TypeParameterListSyntax),
          default(BaseListSyntax),
          default(SyntaxList<TypeParameterConstraintClauseSyntax>),
          default(SyntaxList<MemberDeclarationSyntax>)
        );
    }


    /// <summary>
    /// Yields the Render Method with dynamic ViewModel Parameter:
    /// public static string Render(dynamic viewModel){}
    /// </summary>
    /// <returns></returns>
    internal static MethodDeclarationSyntax RenderWithDynamicParameter()
    {
      return
        SF.MethodDeclaration(
          new SyntaxList<AttributeListSyntax>(),
          SF.TokenList(
            SF.Token(SyntaxKind.PublicKeyword),
            SF.Token(SyntaxKind.StaticKeyword)),
          SF.PredefinedType(SF.Token(SyntaxKind.StringKeyword)),
          default(ExplicitInterfaceSpecifierSyntax),
          SF.Identifier("Render"),
          default(TypeParameterListSyntax),
          SF.ParameterList(new SeparatedSyntaxList<ParameterSyntax>().Add(SyntaxFactory.Parameter(
            default(SyntaxList<AttributeListSyntax>),
            default(SyntaxTokenList),
            SF.ParseTypeName("dynamic"), //no native support for dynamic yet
            SF.Identifier("viewModel"),
            default(EqualsValueClauseSyntax)))
          ),
          default(SyntaxList<TypeParameterConstraintClauseSyntax>),
          SF.Block(),
          default(SyntaxToken)
        );
    }

    /// <summary>
    /// Yields the Render Method without ViewModel Parameter:
    /// public static string Render(){}
    /// </summary>
    internal static MethodDeclarationSyntax RenderWithoutParameter()
    {
      return
        SF.MethodDeclaration(
          new SyntaxList<AttributeListSyntax>(),
          SF.TokenList(
            SF.Token(SyntaxKind.PublicKeyword),
            SF.Token(SyntaxKind.StaticKeyword)),
          SF.PredefinedType(SF.Token(SyntaxKind.StringKeyword)),
          default(ExplicitInterfaceSpecifierSyntax),
          SF.Identifier("Render"),
          default(TypeParameterListSyntax),
          SF.ParameterList(),
          default(SyntaxList<TypeParameterConstraintClauseSyntax>),
          SF.Block(),
          default(SyntaxToken)
        );
    }

    /// <summary>
    /// Yields the Render Method with ViewModel Parameter:
    /// public static string Render(TViewModel viewModel){}
    /// </summary>
    internal static MethodDeclarationSyntax RenderWithParameter(string typeName)
    {
      return
        SF.MethodDeclaration(
          new SyntaxList<AttributeListSyntax>(),
          SF.TokenList(
            SF.Token(SyntaxKind.PublicKeyword),
            SF.Token(SyntaxKind.StaticKeyword)),
          SF.PredefinedType(SF.Token(SyntaxKind.StringKeyword)),
          default(ExplicitInterfaceSpecifierSyntax),
          SF.Identifier("Render"),
          default(TypeParameterListSyntax),
          SF.ParameterList(new SeparatedSyntaxList<ParameterSyntax>().Add(SyntaxFactory.Parameter(
            default(SyntaxList<AttributeListSyntax>),
            default(SyntaxTokenList),
            SF.ParseTypeName(typeName),
            SF.Identifier("viewModel"),
            default(EqualsValueClauseSyntax)))
          ),
          default(SyntaxList<TypeParameterConstraintClauseSyntax>),
          SF.Block(),
          default(SyntaxToken)
        );
    }


    internal static ExpressionStatementSyntax AppendStringLiteral(string value)
    {
      return
        SF.ExpressionStatement(
          SF.InvocationExpression(
            SF.ParseExpression("sb.Append")
          ).AddArgumentListArguments(
            SF.Argument(SF.LiteralExpression(SyntaxKind.StringLiteralExpression, SF.Literal(value)))
          )
        );
    }

    internal static ExpressionStatementSyntax AppendMemberEncoded(string memberName)
    {
      return
        SF.ExpressionStatement(
          SF.InvocationExpression(
            SF.ParseExpression("sb.Append")
          )
          .AddArgumentListArguments(
            SF.Argument(
              SF.InvocationExpression(
                SF.ParseExpression("WebUtility.HtmlEncode")
              )
              .AddArgumentListArguments(
                SF.Argument(SF.ParseExpression(memberName))
              )
            )
          )
        );
    }

    /// <summary>
    /// Yields a "sb.append(memberName)" Statement
    /// </summary>
    /// <param name="memberName"></param>
    /// <returns></returns>
    internal static ExpressionStatementSyntax AppendMember(string memberName)
    {
      return
        SF.ExpressionStatement(
          SF.InvocationExpression(
            SF.ParseExpression("sb.Append")
          ).AddArgumentListArguments(
            SF.Argument(SF.ParseExpression(memberName))
          )
        );
    }

    internal static MethodDeclarationSyntax IsTruthyMethodBool()
    {
      return
        SF.MethodDeclaration(
          new SyntaxList<AttributeListSyntax>(),
          SF.TokenList(
            SF.Token(SyntaxKind.PublicKeyword),
            SF.Token(SyntaxKind.StaticKeyword)),
          SF.PredefinedType(SF.Token(SyntaxKind.BoolKeyword)),
          default(ExplicitInterfaceSpecifierSyntax),
          SF.Identifier("IsTruthy"),
          default(TypeParameterListSyntax),
          SF.ParameterList(new SeparatedSyntaxList<ParameterSyntax>().Add(SyntaxFactory.Parameter(
            default(SyntaxList<AttributeListSyntax>),
            default(SyntaxTokenList),
            SF.PredefinedType(SF.Token(SyntaxKind.BoolKeyword)),
            SF.Identifier("b"),
            default(EqualsValueClauseSyntax)))
          ),
          default(SyntaxList<TypeParameterConstraintClauseSyntax>),
          SF.Block(
            SF.ReturnStatement(SF.ParseExpression("b"))    
          ),
          default(SyntaxToken)
      );
    }

    internal static MethodDeclarationSyntax IsTruthyMethodString()
    {
      return
        SF.MethodDeclaration(
          new SyntaxList<AttributeListSyntax>(),
          SF.TokenList(
            SF.Token(SyntaxKind.PublicKeyword),
            SF.Token(SyntaxKind.StaticKeyword)),
          SF.PredefinedType(SF.Token(SyntaxKind.BoolKeyword)),
          default(ExplicitInterfaceSpecifierSyntax),
          SF.Identifier("IsTruthy"),
          default(TypeParameterListSyntax),
          SF.ParameterList(new SeparatedSyntaxList<ParameterSyntax>().Add(SyntaxFactory.Parameter(
            default(SyntaxList<AttributeListSyntax>),
            default(SyntaxTokenList),
            SF.PredefinedType(SF.Token(SyntaxKind.StringKeyword)),
            SF.Identifier("s"),
            default(EqualsValueClauseSyntax)))
          ),
          default(SyntaxList<TypeParameterConstraintClauseSyntax>),
          SF.Block(
            SF.ReturnStatement(SF.ParseExpression("!string.IsNullOrEmpty(s)"))
          ),
          default(SyntaxToken)
      );
    }

    internal static MethodDeclarationSyntax IsTruthyMethodObject()
    {
      return
        SF.MethodDeclaration(
          new SyntaxList<AttributeListSyntax>(),
          SF.TokenList(
            SF.Token(SyntaxKind.PublicKeyword),
            SF.Token(SyntaxKind.StaticKeyword)),
          SF.PredefinedType(SF.Token(SyntaxKind.BoolKeyword)),
          default(ExplicitInterfaceSpecifierSyntax),
          SF.Identifier("IsTruthy"),
          default(TypeParameterListSyntax),
          SF.ParameterList(new SeparatedSyntaxList<ParameterSyntax>().Add(SyntaxFactory.Parameter(
            default(SyntaxList<AttributeListSyntax>),
            default(SyntaxTokenList),
            SF.PredefinedType(SF.Token(SyntaxKind.ObjectKeyword)),
            SF.Identifier("o"),
            default(EqualsValueClauseSyntax)))
          ),
          default(SyntaxList<TypeParameterConstraintClauseSyntax>),
          SF.Block(
            SF.ReturnStatement(SF.ParseExpression("o!=null"))
          ),
          default(SyntaxToken)
      );
    }

    internal static IfStatementSyntax IfIsTruthy(string memberName, List<StatementSyntax> block)
    {
      return SF.IfStatement(
        SF.ParseExpression($"IsTruthy({memberName})"),
        SF.Block(block)
      );
    }

    internal static StatementSyntax IfIsTruthy(Context currentContext, Context contextToCheck, List<StatementSyntax> block)
    {
      var ifExpression = CheckContextForTruthy(currentContext, contextToCheck);      
      if (ifExpression == null)
        return SF.Block(block);
      return SF.IfStatement(
        ifExpression,
        SF.Block(block)
      );
    }

    internal static IfStatementSyntax IfIsTruthyElse(string memberName, List<StatementSyntax> ifBlock, List<StatementSyntax> elseBlock)
    {
      return IfIsTruthy(memberName, ifBlock)
        .WithElse(
          SF.ElseClause(SF.Block(elseBlock)));
    }

    internal static IfStatementSyntax UnlessIsTruthy(string memberName, List<StatementSyntax> block)
    {
      return SF.IfStatement(
        SF.ParseExpression($"!IsTruthy({memberName})"),
        SF.Block(block)
      );
    }

    internal static IfStatementSyntax UnlessIsTruthyElse(string memberName, List<StatementSyntax> ifBlock, List<StatementSyntax> elseBlock)
    {
      return UnlessIsTruthy(memberName, ifBlock)
        .WithElse(
          SF.ElseClause(SF.Block(elseBlock)));
    }

    internal static StatementSyntax AddCommentToStatement(StatementSyntax statement, string comment)
    {
      return
        statement.WithTrailingTrivia(
          SF.Comment(string.Concat("/*",comment,"*/"))
        );
    }

    /// <summary>
    /// Yields a foreach(var loopVariable in loopedVariable) Statement
    /// </summary>
    /// <param name="loopVariable"></param>
    /// <param name="loopedVariable"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    internal static StatementSyntax ForLoop(string loopVariable, string loopedVariable, List<StatementSyntax> block)
    {
      return
        SF.ForEachStatement(
             SF.ParseTypeName("var"),
             loopVariable,
             SF.ParseExpression(loopedVariable),
             SF.Block(block)
          );
    }

    private static ExpressionSyntax CheckContextForTruthy(Context currentContext, Context contextToCheck)
    {
      var argumentList = new List<string>();
      var pathToCheck = contextToCheck.FullPath;
      if (contextToCheck.FullPath.StartsWith(currentContext.FullPath)
          && currentContext.FullPath.Contains("."))
      {//The context to check is directly depended from the currentContext
        if (currentContext.FullPath.Equals(contextToCheck.FullPath))
          return null;
        pathToCheck = contextToCheck.FullPath.Substring(currentContext.FullPath.Length + 1);
        var elements = pathToCheck.Split('.');
        for(int i = 1;i<=elements.Length;i++)
        {
          argumentList.Add(string.Join(".", currentContext.FullPath, string.Join(".", elements.Take(i).ToArray())));
        }
      } else
      {//The context to check is independed from the currentContext
        var elements = pathToCheck.Split('.');
        for (int i = 1; i <= elements.Length; i++)
        {
          argumentList.Add(string.Join(".", elements.Take(i).ToArray()));
        }
      }
      var result = SF.ParseExpression($"IsTruthy({argumentList[0]})");
      foreach(var element in argumentList.Skip(1))
        result = BinaryIfIsTruthyExpression(result, element);      
      return result;
    }

    /// <summary>
    /// Yields IsTruthy(a) && IsTruthy(b)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static BinaryExpressionSyntax BinaryIfIsTruthyExpression(string a, string b)
    {
      return SF.BinaryExpression(SyntaxKind.LogicalAndExpression,
        SF.ParseExpression($"IsTruthy({a})"),
        SF.ParseExpression($"IsTruthy({b})")
      );
    }

    /// <summary>
    /// Yields IsTruthy(a) && IsTruthy(b)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static BinaryExpressionSyntax BinaryIfIsTruthyExpression(ExpressionSyntax a, string b)
    {
      return SF.BinaryExpression(SyntaxKind.LogicalAndExpression,
        a,
        SF.ParseExpression($"IsTruthy({b})")
      );
    }

  }

}
