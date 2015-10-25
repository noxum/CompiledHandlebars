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
        SF.UsingDirective(SF.ParseName("System.Net")),
        SF.UsingDirective(SF.ParseName("System"))
      };


    internal static UsingDirectiveSyntax UsingStatic(string name)
    {
      return SF.UsingDirective(SF.Token(SyntaxKind.StaticKeyword), default(NameEqualsSyntax), SF.ParseName(name));
    }

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
          new SyntaxList<AttributeListSyntax>().Add(
          SF.AttributeList(new SeparatedSyntaxList<AttributeSyntax>().Add(
            SF.Attribute(SF.ParseName("CompiledHandlebarsTemplate")))
          )),
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
          SF.ParameterList(new SeparatedSyntaxList<ParameterSyntax>().Add(SF.Parameter(
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

    /// <summary>
    /// sb.Append(WebUtility.HtmlEncode(memberName))
    /// </summary>
    /// <param name="memberName"></param>
    /// <returns></returns>
    internal static ExpressionStatementSyntax AppendMemberEncoded(string memberName, bool isString)
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
                SF.Argument(SF.ParseExpression(isString ? memberName : $"{memberName}.ToString()"))
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
    internal static ExpressionStatementSyntax AppendMember(string memberName, bool isString)
    {
      return
        SF.ExpressionStatement(
          SF.InvocationExpression(
            SF.ParseExpression("sb.Append")
          ).AddArgumentListArguments(
            SF.Argument(SF.ParseExpression(isString?memberName:$"{memberName}.ToString()"))
          )
        );
    }


    /// <summary>
    /// private static bool IsTruthy(bool b)
    /// </summary>
    /// <returns></returns>
    internal static MethodDeclarationSyntax IsTruthyMethodBool()
    {
      return
        SF.MethodDeclaration(
          new SyntaxList<AttributeListSyntax>(),
          SF.TokenList(
            SF.Token(SyntaxKind.PrivateKeyword),
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

    /// <summary>
    /// private static bool IsTruthy(string s)
    /// </summary>
    /// <returns></returns>
    internal static MethodDeclarationSyntax IsTruthyMethodString()
    {
      return
        SF.MethodDeclaration(
          new SyntaxList<AttributeListSyntax>(),
          SF.TokenList(
            SF.Token(SyntaxKind.PrivateKeyword),
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

    /// <summary>
    /// private static bool IsTruthy(object o)
    /// </summary>
    /// <returns></returns>
    internal static MethodDeclarationSyntax IsTruthyMethodObject()
    {
      return
        SF.MethodDeclaration(
          new SyntaxList<AttributeListSyntax>(),
          SF.TokenList(
            SF.Token(SyntaxKind.PrivateKeyword),
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

    /// <summary>
    /// sb.Append(Template.Render(membername))
    /// </summary>
    /// <param name="templateTypeName"></param>
    /// <param name="memberName"></param>
    /// <returns></returns>
    internal static ExpressionStatementSyntax PartialTemplateCall(string templateTypeName, string memberName)
    {
      return
        SF.ExpressionStatement(
          SF.InvocationExpression(
            SF.ParseExpression("sb.Append")
          )
          .AddArgumentListArguments(
            SF.Argument(
              SF.InvocationExpression(
                SF.ParseExpression($"{templateTypeName}.Render")
              ).AddArgumentListArguments(
                SF.Argument(SF.ParseExpression(memberName))
              )
            )
          )
        );
    }

    internal static ExpressionStatementSyntax SelfReferencingPartialCall(string memberName)
    {
      return
       SF.ExpressionStatement(
         SF.InvocationExpression(
           SF.ParseExpression("sb.Append")
         )
         .AddArgumentListArguments(
           SF.Argument(
             SF.InvocationExpression(
               SF.ParseExpression("Render")
             ).AddArgumentListArguments(
               SF.Argument(SF.ParseExpression(memberName))
             )
           )
         )
       );
    }

    /// <summary>
    /// private class CompiledHandlebarsTemplateAttribute : Attribute
    /// </summary>
    /// <returns></returns>
    internal static ClassDeclarationSyntax CompiledHandlebarsTemplateAttributeClass()
    {
      return
        SF.ClassDeclaration(
            default(SyntaxList<AttributeListSyntax>),
            SF.TokenList(
              SF.Token(SyntaxKind.PrivateKeyword)),
            SF.Identifier("CompiledHandlebarsTemplateAttribute"),
            default(TypeParameterListSyntax),
            SF.BaseList(new SeparatedSyntaxList<BaseTypeSyntax>().Add(
              SF.SimpleBaseType(SF.ParseTypeName("Attribute")))),
            default(SyntaxList<TypeParameterConstraintClauseSyntax>),
            default(SyntaxList<MemberDeclarationSyntax>)
          );
    }


    internal static IfStatementSyntax IfIsTruthy(List<string> elementsToCheck, AST.IfType ifType)
    {
      var condition = CheckContextForTruthy(elementsToCheck, ifType);
      if (condition == null)
        return null;
      else return SF.IfStatement(condition, SF.EmptyStatement());
    }


    internal static StatementSyntax EmptyStatementWithComment(string comment)
    {      
      return
        SF.EmptyStatement(SF.Token(SyntaxKind.SemicolonToken)).WithTrailingTrivia(
          SF.Comment(string.Concat("/*", comment, "*/")));
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

    /// <summary>
    /// Concats elements to a condition (e.g. (a && b && c) or (!a || !b || !c))
    /// </summary>
    /// <param name="elementsToCheck"></param>
    /// <param name="ifType"></param>
    /// <returns></returns>
    private static ExpressionSyntax CheckContextForTruthy(List<string> elementsToCheck, AST.IfType ifType)
    {
      if (elementsToCheck == null || !elementsToCheck.Any())
        return null;
      var result = ifType == AST.IfType.If ? SF.ParseExpression($"IsTruthy({elementsToCheck[0]})")
                                           : SF.ParseExpression($"!IsTruthy({elementsToCheck[0]})");
      foreach (var element in elementsToCheck.Skip(1))
        result = ifType == AST.IfType.If ? BinaryIfIsTruthyExpression(result, element)
                                      : BinaryUnlessIsTruthyExpression(result, element);
      return result;
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
    /// <summary>
    /// Yields !IsTruthy(a) || !IsTruthy(b)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private static BinaryExpressionSyntax BinaryUnlessIsTruthyExpression(ExpressionSyntax a, string b)
    {
      return SF.BinaryExpression(SyntaxKind.LogicalOrExpression,
        a,
        SF.ParseExpression($"!IsTruthy({b})")
      );
    }

  }

}
