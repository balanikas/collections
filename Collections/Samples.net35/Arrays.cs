using System.Collections;
using System.Collections.Generic;

namespace Samples
{
    internal class Arrays
    {
        public static void UsingArray()
        {
            var array = new int[100];
            for (int i = 0; i < 100; i++)
            {
                array[i] = i;
            }
        }

        public static void UsingArrayList()
        {
            var array = new ArrayList(100);
            for (int i = 0; i < 100; i++)
            {
                array.Add(i);
            }
        }

        public static void UsingGenericList()
        {
            var array = new List<int>(100);
            for (int i = 0; i < 100; i++)
            {
                array.Add(i);
            }
        }

        public static void UsingGenericLinkedList()
        {
            var array = new LinkedList<int>();
            for (int i = 0; i < 100; i++)
            {
                array.AddLast(i);
            }
        }
    }
}