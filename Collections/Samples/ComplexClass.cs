﻿using System;
using System.Collections.Generic;

namespace Samples
{
    namespace B
    {
        [Serializable]
        public abstract class X
        {
            protected string message = "from base class";

            public X()
            {
                var myList = new List<string>();
            }
        }

        [Serializable]
        public class ComplexClass : X
        {
            public ComplexClass()
            {
                var myList = new List<string>();
                for (int i = 0; i < 10; i++)
                {
                    myList.Add(i.ToString());
                    bool contains = myList.Contains(i.ToString());
                    myList.Clear();
                }
            }

            public ComplexClass(int number)
            {
            }
        }
    }
}