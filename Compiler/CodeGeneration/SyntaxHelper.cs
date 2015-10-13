﻿using Microsoft.CodeAnalysis;
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

  }

}
