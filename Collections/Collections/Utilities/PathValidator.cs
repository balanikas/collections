using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections.Utilities
{
    public class PathValidator
    {
        public enum PathType
        {
            SourceFile,
            SourceFolder,
            AssemblyFile,
            Unknown
        }
        public static PathType DeterminePathType(string path)
        {
            if (IsValidSourceFilePath(path))
            {
                return PathType.SourceFile;
            }
            else if (IsValidSourceFolderPath(path))
            {
                return PathType.SourceFolder;
            }
            else if (IsValidAssemblyPath(path))
            {
                return PathType.AssemblyFile;
            }
            else
            {
                return PathType.Unknown;
            }
        }

        public static bool IsValidSourceFolderPath(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            if (!Directory.EnumerateFiles(path, "*.cs", SearchOption.TopDirectoryOnly).Any())
            {
                return false;
            }
            
            return true;
        }

        public static bool IsValidSourceFilePath(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            return path.EndsWith(".cs");
        }

        public static bool IsValidAssemblyPath(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            return path.EndsWith(".dll") || path.EndsWith(".exe");
        }

        public static bool IsWellFormedPath(string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                return false;
            }

            if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                return false;
            }
            return true;
        }


    }
}
