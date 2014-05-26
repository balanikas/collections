using System.Collections.Generic;
using System.Reflection;

namespace Collections
{
    public class LoadedType
    {
        public TypeInfo TypeInfo { get; set; }
        public List<MethodInfo> MethodsInfos { get; set; }
       
        public string FilePath { get; set; }
        public string Source { get; set; }
        public bool IsCompilable { get; set; }
        public bool AllowEditSource { get; set; }
    }
}