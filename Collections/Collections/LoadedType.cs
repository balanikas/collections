using System;
using System.Collections.Generic;
using System.Reflection;

namespace Collections
{
    public class LoadedType
    {
        public TypeInfo TypeInfo { get; private set; }
        public List<MethodInfo> MethodsInfos { get; private set; }
       
        public string FilePath { get; set; }
        public string Source { get; private set; }
        public bool AllowEditSource { get { return !String.IsNullOrEmpty(Source); }}

        public LoadedType(TypeInfo typeInfo, string filePath, string source )
        {
            TypeInfo = typeInfo;
            MethodsInfos = new List<MethodInfo>(typeInfo.GetMethods(
                BindingFlags.Instance | 
                BindingFlags.Static | 
                BindingFlags.DeclaredOnly | 
                BindingFlags.NonPublic |
                BindingFlags.Public));

            Source = source;
            FilePath = filePath;

        }
    }
}