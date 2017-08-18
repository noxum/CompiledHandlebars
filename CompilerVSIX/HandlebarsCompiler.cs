using CompiledHandlebars.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace CompiledHandleBarsCompilerVSIX
{
	[ComVisible(true)]
	[Guid("F84ABD20-9386-40D5-A4A0-4443B7206EC5")]
	[CodeGeneratorRegistration(
	typeof(HandlebarsCompiler),
		"HandlebarsCompiler",
		vsContextGuidVCSProject,
		GeneratesDesignTimeSource = true)]
	[ProvideObject(typeof(HandlebarsCompiler),
		RegisterUsing = RegistrationMethod.CodeBase)]
	public class HandlebarsCompiler : IVsSingleFileGenerator
	{
		public const string vsContextGuidVCSProject = "{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}";

		#region IVsSingleFileGenerator

		public int DefaultExtension(out string pbstrDefaultExtension)
		{
			pbstrDefaultExtension = ".hbs.cs";
			return VSConstants.S_OK;
		}

		public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
		{
#if DEBUG
			var sw = new Stopwatch();
			sw.Start();
#endif
			var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
			var workspace = (Workspace)componentModel.GetService<VisualStudioWorkspace>();
			var docIds = workspace.CurrentSolution.GetDocumentIdsWithFilePath(wszInputFilePath);
			var project = FindContainingProject(workspace.CurrentSolution.Projects.ToList(), wszInputFilePath);
			var compilationResult = HbsCompiler.Compile(bstrInputFileContents, wszDefaultNamespace, Path.GetFileNameWithoutExtension(wszInputFilePath), project);
#if DEBUG
			sw.Stop();
#endif
			if (compilationResult.Item2.Any())
			{
				foreach (var error in compilationResult.Item2)
					pGenerateProgress.GeneratorError(0, 1, error.Message, (uint)error.Line - 1, (uint)error.Column - 1);
			}
			byte[] bytes = Encoding.UTF8.GetBytes(compilationResult.Item1);
			rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(bytes.Length);
			Marshal.Copy(bytes, 0, rgbOutputFileContents[0], bytes.Length);
			pcbOutput = (uint)bytes.Length;
			return compilationResult.Item2.Any() ? VSConstants.E_FAIL : VSConstants.S_OK;
		}

		#endregion

		private Project FindContainingProject(List<Project> projects, string filePath)
		{
			foreach (var project in projects)
			{
				//Assumption: The file is in the directory of the project...
				//This assumption is probably not always true. So:
				//TODO: Find a way to solve this problem correctly
				var dirPath = Path.GetDirectoryName(project.FilePath);
				if (filePath.StartsWith(dirPath))
					return project;
			}
			return null;
		}
	}
}
