using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler
{
  public static class NamespaceUtility
  {

    public static bool IsPartOf(string fullNamespace, string partNamespace)
    {
      var partParts = partNamespace.Split('.').Reverse().ToList();
      var fullParts = fullNamespace.Split('.').Reverse().ToList();
      Debug.Assert(partParts.Count <= fullParts.Count, "PartNamespace is longer than FullNamespace");
      if (partParts.Count > fullParts.Count)
        return false;
      for(int i = 0; i< partParts.Count; i++)
      {
        if (!partParts[i].Equals(fullParts[i]))
          return false;
      }
      return true;
    }


  }
}
