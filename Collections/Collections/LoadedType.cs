using System.Reflection;

namespace Collections
{
    public class LoadedType
    {
        public TypeInfo TypeInfo { get; set; }
        public string FilePath { get; set; }
        public string Source { get; set; }
        public bool IsCompilable { get; set; }
    }
}