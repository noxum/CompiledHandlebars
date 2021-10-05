using CompiledHandlebars.Compiler.Introspection;
using CompiledHandlebars.Compiler.Visitors;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace CompiledHandlebars.Compiler
{
	public static class HbsCompiler
	{
		private static readonly Regex mandatoryNameSpacePrefixPattern = new Regex(@"^(?<mandatoryNameSpacePrefix>.+?\.Views.*?)\..*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		
		public static Tuple<string, IEnumerable<HandlebarsException>> Compile(string content, string @namespace, string name, Project containingProject)
		{
			var parser = new HbsParser();
			var template = parser.Parse(content != null ? content.Replace("\r\n", "\n") : content);
			template.Namespace = @namespace;
			template.Name = name;
			if (!(template.ParseErrors?.Any() ?? false))
			{//No parser errors
				Match m = mandatoryNameSpacePrefixPattern.Match(@namespace);
				string mandatoryNamespacePrefix = null;
				if (m.Success)
					mandatoryNamespacePrefix = m.Groups["mandatoryNameSpacePrefix"].Value + ".";
				var codeGenerator = new CodeGenerationVisitor(new RoslynIntrospector(containingProject, mandatoryNamespacePrefix), template);
				if (!codeGenerator.ErrorList.Any())
				{//No code generator initialization errors
					return new Tuple<string, IEnumerable<HandlebarsException>>(
					  codeGenerator.GenerateCode().NormalizeWhitespace(indentation: "\t").ToFullString(), codeGenerator.ErrorList);
				}
				return new Tuple<string, IEnumerable<HandlebarsException>>(string.Empty, codeGenerator.ErrorList);
			}
			return new Tuple<string, IEnumerable<HandlebarsException>>(string.Empty, template.ParseErrors);
		}
	}
}
