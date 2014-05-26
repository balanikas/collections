using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    class ArraysInParams
    {
        public int GetArrayLength(byte[] bytes)
        {
            return bytes.Length;
        }

        public int GetArrayLength(string[] strings)
        {
            return strings.Length;
        }

    }
}
