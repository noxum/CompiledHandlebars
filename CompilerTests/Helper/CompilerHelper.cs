using CompiledHandlebars.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.Helper
{
  public static class CompilerHelper
  {
    public static Assembly CompileTemplatesToAssembly(Type testClassType)
    {

      var solutionFile = Path.Combine(Directory.CreateDirectory(Environment.CurrentDirectory).Parent.Parent.Parent.FullName, "CompiledHandlebars.sln");
      List<SyntaxTree> compiledTemplates = new List<SyntaxTree>();
      var workspace = MSBuildWorkspace.Create();
      var sol = workspace.OpenSolutionAsync(solutionFile).Result;
      foreach (MethodInfo methodInfo in (testClassType).GetMethods())
      {
        var attrList = methodInfo.GetCustomAttributes(typeof(RegisterHandlebarsTemplateAttribute), false) as RegisterHandlebarsTemplateAttribute[];
        foreach (var template in attrList)
        {//Get compiled templates
          var code = HbsCompiler.Compile(template._contents, "TestTemplates", template._name, workspace).Item1;
          compiledTemplates.Add(CSharpSyntaxTree.ParseText(code));
        }
      }

      string assemblyName = Path.GetRandomFileName();
      MetadataReference[] references = new MetadataReference[]
      {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(WebUtility).Assembly.Location),
        MetadataReference.CreateFromFile(testClassType.Assembly.Location),
      };
      var compilation = CSharpCompilation.Create(
        assemblyName,
        compiledTemplates,
        references,
        options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

      using (var ms = new MemoryStream())
      {
        var emitResult = compilation.Emit(ms);

        if (!emitResult.Success)
        {
          IEnumerable<Diagnostic> failures = emitResult.Diagnostics.Where(diagnostic =>
                          diagnostic.IsWarningAsError ||
                          diagnostic.Severity == DiagnosticSeverity.Error);
          throw new Exception(failures.First().GetMessage());
        }
        ms.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(ms.ToArray());
      }
    }
  }
}
