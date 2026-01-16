using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CompiledHandlebars.Compiler
{
    public static class NamespaceUtility
    {
        public static bool IsPartOf(string fullNamespace, string partNamespace)
        {
            List<string> partParts = partNamespace.Split('.').AsEnumerable().Reverse().ToList();
            List<string> fullParts = fullNamespace.Split('.').AsEnumerable().Reverse().ToList();
            Debug.Assert(partParts.Count <= fullParts.Count, "PartNamespace is longer than FullNamespace");
            if (partParts.Count > fullParts.Count)
                return false;
            for (int i = 0; i < partParts.Count; i++)
            {
                if (!partParts[i].Equals(fullParts[i]))
                    return false;
            }
            return true;
        }
    }
}
