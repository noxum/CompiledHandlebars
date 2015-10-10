using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
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
    internal static NamespaceDeclarationSyntax HandlebarsNamespace(string nameSpace)
    {
      return SF.NamespaceDeclaration(SF.ParseName(nameSpace));
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
  }
}
