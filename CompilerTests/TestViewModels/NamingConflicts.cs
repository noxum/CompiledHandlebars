using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.CompilerTests.TestViewModels
{
  public class NamingConflictsBaseModel
  {
    public List<string> Items { get; set; }
    public string ItemsTitle { get; set; }
  }

  public class NamingConflictsModel : NamingConflictsBaseModel
  {
    public bool ItemsUseLinkTitle { get; set; }
  }
}
