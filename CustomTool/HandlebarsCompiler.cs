using CompiledHandlebars.Compiler;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.LanguageServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using VSLangProj80;

namespace CompiledHandlebars.CustomTool
{

  /// <summary>
  /// This class contains the boilerplate code for interfacing with VisualStudio as CustomTool.
  /// There is few to no documentation on how this works. This Code is mostly taken from the RazorGenerator project:
  /// https://github.com/RazorGenerator/RazorGenerator/blob/master/RazorGenerator.Tooling/RazorGenerator.cs
  /// </summary>
  [ComVisible(true)]
  [Guid("F84ABD20-9386-40D5-A4A0-4443B7206EC5")]
  [CodeGeneratorRegistration(
    typeof(HandlebarsCompiler),
    "HandlebarsCompiler",
    vsContextGuids.vsContextGuidVCSProject,
    GeneratesDesignTimeSource = true)]
  public class HandlebarsCompiler : IVsSingleFileGenerator, IObjectWithSite
  {
    private object site = null;

    private ServiceProvider serivceProvider;
    private ServiceProvider siteServiceProvider
    {
      get
      {
        if (serivceProvider == null)
        {
          serivceProvider = new ServiceProvider(site as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);
        }
        return serivceProvider;
      }
    }


    public int DefaultExtension(out string pbstrDefaultExtension)
    {
      pbstrDefaultExtension = ".hbs.cs";
      return VSConstants.S_OK;
    }

    public int Generate(string wszInputFilePath, string bstrInputFileContents, string wszDefaultNamespace, IntPtr[] rgbOutputFileContents, out uint pcbOutput, IVsGeneratorProgress pGenerateProgress)
    {
      var sw = new Stopwatch();
      sw.Start();
      var componentModel = (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
      var workspace = componentModel.GetService<VisualStudioWorkspace>();
      var docIds = workspace.CurrentSolution.GetDocumentIdsWithFilePath(wszInputFilePath);
      var project = workspace.CurrentSolution.GetDocument(docIds.First()).Project;
      var compilationResult = HbsCompiler.Compile(bstrInputFileContents, wszDefaultNamespace, Path.GetFileNameWithoutExtension(wszInputFilePath), project);
      sw.Stop();      
      if (compilationResult.Item2.Any())
      {
        foreach(var error in compilationResult.Item2)        
          pGenerateProgress.GeneratorError(0, 1, error.Message, (uint)error.Line, (uint)error.Column);
      }
      byte[] bytes = Encoding.UTF8.GetBytes(string.Concat(compilationResult.Item1, $"/*compiled in {sw.ElapsedMilliseconds}ms*/"));
      rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(bytes.Length);
      Marshal.Copy(bytes, 0, rgbOutputFileContents[0], bytes.Length);
      pcbOutput = (uint)bytes.Length;
      return compilationResult.Item2.Any()?VSConstants.E_FAIL:VSConstants.S_OK;
    }

    public void GetSite(ref Guid riid, out IntPtr ppvSite)
    {
      if (site == null)
      {
        throw new COMException("object is not sited", VSConstants.E_FAIL);
      }

      IntPtr pUnknownPointer = Marshal.GetIUnknownForObject(site);
      IntPtr intPointer = IntPtr.Zero;
      Marshal.QueryInterface(pUnknownPointer, ref riid, out intPointer);

      if (intPointer == IntPtr.Zero)
      {
        throw new COMException("site does not support requested interface", VSConstants.E_NOINTERFACE);
      }

      ppvSite = intPointer;
    }

    public void SetSite(object pUnkSite)
    {
      site = pUnkSite;
    }
  }
}
