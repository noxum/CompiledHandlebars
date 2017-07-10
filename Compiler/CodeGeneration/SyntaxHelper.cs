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
        /// Yields "sb.Append(argument)"
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private static ExpressionStatementSyntax SbAppend(ArgumentSyntax argument, bool encoded)
        {
            if (encoded)
                argument = EncodeArgument(argument);
            return
              SF.ExpressionStatement(
                SF.InvocationExpression(
                  SF.ParseExpression("sb.Append")
                )
                .AddArgumentListArguments(
                  argument
                )
              );
        }

        /// <summary>
        /// Yields "sb.Append(WebUtility.HtmlEncode(argument))"
        /// </summary>
        /// <param name="argument"></param>
        /// <returns></returns>
        private static ArgumentSyntax EncodeArgument(ArgumentSyntax argument)
        {
            return
              SF.Argument(
                SF.InvocationExpression(
                  SF.ParseExpression("WebUtility.HtmlEncode")
                )
                .AddArgumentListArguments(
                  argument
                )
              );
        }

        private static IdentifierNameSyntax ToStringIdentifierName = SF.IdentifierName("ToString()");

        /// <summary>
        /// Yields "expression.ToString()"
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static MemberAccessExpressionSyntax ExpressionToString(ExpressionSyntax expression)
        {
            return
              SF.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                expression,
                ToStringIdentifierName
              );
        }

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
                      SF.ArgumentList(
                        new SeparatedSyntaxList<ArgumentSyntax>().Add(
                          SF.Argument(SF.ParseExpression("64")))
                        ),
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


        internal static UsingDirectiveSyntax UsingStatic(string name)
        {
            return SF.UsingDirective(SF.Token(SyntaxKind.StaticKeyword), default(NameEqualsSyntax), SF.ParseName(name));
        }

        /// <summary>
        /// Yields a NamespaceDeclaration:
        /// namespace CompiledHandlebars{}
        /// </summary>
        internal static NamespaceDeclarationSyntax HandlebarsNamespace(string @namespace)
        {
            return SF.NamespaceDeclaration(SF.ParseName(@namespace));
        }


        /// <summary>
        /// Yields the CompiledHandlebars Class Declaration ClassDeclaration
        /// public static class CompiledHandlebarsTemplate<TViewModel> {}
        /// </summary>
        internal static ClassDeclarationSyntax CompiledHandlebarsClassDeclaration(string templateName, string attribute)
        {
            return
              SF.ClassDeclaration(
                new SyntaxList<AttributeListSyntax>().Add(
                SF.AttributeList(new SeparatedSyntaxList<AttributeSyntax>().Add(
                  SF.Attribute(SF.ParseName(attribute)))
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
        internal static MethodDeclarationSyntax RenderWithParameter(string typeName, string methodName = "Render")
        {
            return
              SF.MethodDeclaration(
                new SyntaxList<AttributeListSyntax>(),
                SF.TokenList(
                  SF.Token(SyntaxKind.PublicKeyword),
                  SF.Token(SyntaxKind.StaticKeyword)),
                SF.PredefinedType(SF.Token(SyntaxKind.StringKeyword)),
                default(ExplicitInterfaceSpecifierSyntax),
                SF.Identifier(methodName),
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


        /// <summary>
        /// Yields the Render Method:
        /// public static string Render(){}
        /// </summary>
        internal static MethodDeclarationSyntax RenderWithoutParameter(string methodName = "Render")
        {
            return
              SF.MethodDeclaration(
                new SyntaxList<AttributeListSyntax>(),
                SF.TokenList(
                  SF.Token(SyntaxKind.PublicKeyword),
                  SF.Token(SyntaxKind.StaticKeyword)),
                SF.PredefinedType(SF.Token(SyntaxKind.StringKeyword)),
                default(ExplicitInterfaceSpecifierSyntax),
                SF.Identifier(methodName),
                default(TypeParameterListSyntax),
                SF.ParameterList(),
                default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                SF.Block(),
                default(SyntaxToken)
              );
        }

        internal static ExpressionStatementSyntax AppendStringLiteral(string value)
        {
            return
              SbAppend(
                SF.Argument(SF.LiteralExpression(SyntaxKind.StringLiteralExpression, SF.Literal(value))),
                encoded: false
              );
        }

        /// <summary>
        /// Yields a "sb.append(memberName)" Statement
        /// </summary>
        /// <param name="memberName"></param>
        /// <returns></returns>
        internal static ExpressionStatementSyntax AppendMember(string memberName, bool isString, bool encoded)
        {
            return
              SbAppend(
                  SF.Argument(SF.ParseExpression(isString ? memberName : $"{memberName}.ToString()")),
                  encoded: encoded
                );
        }

        /// <summary>
        /// Yields "sb.Append(functionName(parameter1, parameter2))" or 
        /// "sb.Append(functionName(parameter1, parameter2).ToString())"
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        internal static ExpressionStatementSyntax AppendFuntionCallResult(string functionName, IList<string> parameters, bool returnTypeIsString, bool encoded)
        {
            if (returnTypeIsString)
            {
                return
                  SbAppend(
                    SF.Argument(
                      SF.InvocationExpression(SF.ParseExpression(functionName))
                      .AddArgumentListArguments(
                        parameters.Select(x => SF.Argument(SF.ParseExpression(x))).ToArray()
                      )
                    ),
                    encoded: encoded
                  );
            }
            else
            {
                return
                  SbAppend(
                    SF.Argument(
                      ExpressionToString(
                        SF.InvocationExpression(SF.ParseExpression(functionName))
                        .AddArgumentListArguments(
                          parameters.Select(x => SF.Argument(SF.ParseExpression(x))).ToArray()
                        )
                      )
                    ),
                    encoded: encoded
                  );
            }
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
        ///     public static bool IsTruthy<T>(IEnumerable<T> ie)
        /// </summary>
        /// <returns></returns>
        internal static MethodDeclarationSyntax IsTruthyMethodIEnumerableT()
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
                SF.TypeParameterList(new SeparatedSyntaxList<TypeParameterSyntax>().Add(
                  SF.TypeParameter("T"))
                ),
                SF.ParameterList(new SeparatedSyntaxList<ParameterSyntax>().Add(SyntaxFactory.Parameter(
                  default(SyntaxList<AttributeListSyntax>),
                  default(SyntaxTokenList),
                  SF.ParseTypeName("IEnumerable<T>"),
                  SF.Identifier("ie"),
                  default(EqualsValueClauseSyntax)))
                ),
                default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                SF.Block(
                  SF.ReturnStatement(SF.ParseExpression("ie!=null && ie.Any()"))
                ),
                default(SyntaxToken)
            );
        }

        /// <summary>
        ///   public static bool IsTruthy(int i)
        /// </summary>
        /// <returns></returns>
        internal static MethodDeclarationSyntax IsTruthyMethodInt()
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
                  SF.PredefinedType(SF.Token(SyntaxKind.IntKeyword)),
                  SF.Identifier("i"),
                  default(EqualsValueClauseSyntax)))
                ),
                default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                SF.Block(
                  SF.ReturnStatement(SF.ParseExpression("i!=0"))
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
        internal static ExpressionStatementSyntax HbsTemplateCall(string templateTypeName, string memberName, string methodName = "Render")
        {
            return
              SF.ExpressionStatement(
                SF.InvocationExpression(
                  SF.ParseExpression("sb.Append")
                )
                .AddArgumentListArguments(
                  SF.Argument(
                    SF.InvocationExpression(
                      SF.ParseExpression($"{templateTypeName}.{methodName}")
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
                  SF.Identifier(StringConstants.TEMPLATEATTRIBUTEFULL),
                  default(TypeParameterListSyntax),
                  SF.BaseList(new SeparatedSyntaxList<BaseTypeSyntax>().Add(
                    SF.SimpleBaseType(SF.ParseTypeName("Attribute")))),
                  default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                  default(SyntaxList<MemberDeclarationSyntax>)
                );
        }

        /// <summary>
        /// private class CompiledHandlebarsTemplateAttribute : Attribute
        /// </summary>
        /// <returns></returns>
        internal static ClassDeclarationSyntax CompiledHandlebarsLayoutAttributeClass()
        {
            return
              SF.ClassDeclaration(
                  default(SyntaxList<AttributeListSyntax>),
                  SF.TokenList(
                    SF.Token(SyntaxKind.PrivateKeyword)),
                  SF.Identifier(StringConstants.LAYOUTATTRIBUTEFULL),
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

        internal static IfStatementSyntax IfEquals(List<string> elementsToCheck)
        {
            var condition = CheckContextForEquality(elementsToCheck);
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
        internal static ForEachStatementSyntax ForLoop(string loopVariable, string loopedVariable, List<StatementSyntax> block)
        {
            return
              SF.ForEachStatement(
                   SF.ParseTypeName("var"),
                   loopVariable,
                   SF.ParseExpression(loopedVariable),
                   SF.Block(block)
                );
        }

        internal static List<StatementSyntax> PrepareForLoop(AST.EachBlock.ForLoopFlags flags, int loopLevel)
        {
            var result = new List<StatementSyntax>();
            if (flags.HasFlag(AST.EachBlock.ForLoopFlags.Last) || flags.HasFlag(AST.EachBlock.ForLoopFlags.Index))
                result.Add(DeclareIntVariable($"index{loopLevel}"));
            if (flags.HasFlag(AST.EachBlock.ForLoopFlags.Last))
                result.Add(DeclareBoolVariableInitialyFalse($"last{loopLevel}"));
            if (flags.HasFlag(AST.EachBlock.ForLoopFlags.First))
                result.Add(DeclareBoolVariableInitialyTrue($"first{loopLevel}"));
            return result;
        }

        /// <summary>
        /// Yields "bool name;"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static LocalDeclarationStatementSyntax DeclareBoolVariableInitialyFalse(string name)
        {
            return
              SF.LocalDeclarationStatement(
                SF.VariableDeclaration(
                  SF.PredefinedType(SF.Token(SyntaxKind.BoolKeyword)),
                  new SeparatedSyntaxList<VariableDeclaratorSyntax>().Add(
                    SF.VariableDeclarator(SF.Identifier(name), default(BracketedArgumentListSyntax),
                    SF.EqualsValueClause(SF.ParseExpression("false"))
                    )
                  )
                )
              );
        }


        /// <summary>
        /// Yields "bool name;"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static LocalDeclarationStatementSyntax DeclareIntVariable(string name)
        {
            return
              SF.LocalDeclarationStatement(
                SF.VariableDeclaration(
                  SF.PredefinedType(SF.Token(SyntaxKind.IntKeyword)),
                  new SeparatedSyntaxList<VariableDeclaratorSyntax>().Add(
                    SF.VariableDeclarator(SF.Identifier(name), default(BracketedArgumentListSyntax),
                    SF.EqualsValueClause(SF.ParseExpression("0")))
                  )
                )
              );
        }

        /// <summary>
        /// Yields "bool name = true;"
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static LocalDeclarationStatementSyntax DeclareBoolVariableInitialyTrue(string name)
        {
            return
              SF.LocalDeclarationStatement(
                SF.VariableDeclaration(
                  SF.PredefinedType(SF.Token(SyntaxKind.BoolKeyword)),
                  new SeparatedSyntaxList<VariableDeclaratorSyntax>().Add(
                    SF.VariableDeclarator(SF.Identifier(name), default(BracketedArgumentListSyntax),
                    SF.EqualsValueClause(SF.ParseExpression("true"))
                    )
                  )
                )
              );
        }

        /// <summary>
        /// Yields "variable = false;"
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        internal static ExpressionStatementSyntax AssignFalse(string variable)
        {
            return
              SF.ExpressionStatement(
                SF.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                  SF.ParseExpression(variable),
                  SF.ParseExpression($"false")
                )
              );
        }

        /// <summary>
        /// Yields "variable = value==0;"
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static ExpressionStatementSyntax AssignValueEqualsZero(string variable, string value)
        {
            return
              SF.ExpressionStatement(
                SF.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                  SF.ParseExpression(variable),
                  SF.ParseExpression($"{value}==0")
                )
              );
        }

        /// <summary>
        /// Yields "variable = lhs==rhs;"
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static ExpressionStatementSyntax AssignValueEqualsValue(string variable, string lhs, string rhs)
        {

            return
              SF.ExpressionStatement(
                SF.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                  SF.ParseExpression(variable),
                  SF.ParseExpression($"{lhs}=={rhs}")
                )
              );
        }

        /// <summary>
        /// Yields "variable++;"
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        internal static ExpressionStatementSyntax IncrementVariable(string variable)
        {
            return
              SF.ExpressionStatement(
                SF.PostfixUnaryExpression(SyntaxKind.PostIncrementExpression, SF.ParseExpression(variable))
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
        /// </summary>
        /// <param name="elementsToCheck"></param>
        /// <returns></returns>
        private static ExpressionSyntax CheckContextForEquality(List<string> elementsToCheck)
        {
            if (elementsToCheck == null || elementsToCheck.Count != 2)
                return null;
            return SF.ParseExpression($"IsEqual({elementsToCheck[0]},{elementsToCheck[1]})");
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
