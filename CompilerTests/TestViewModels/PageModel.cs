using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.TestViewModels
{
  public interface IPageModel
  {
    string Title { get; set; }    
  }
  public class PageModel : IPageModel
  {
    public string Title { get; set; }
    public string Headline { get; set; }
  }

  public class PageListModel
  {
    public List<PageModel> Items { get; set; }
  }
}
