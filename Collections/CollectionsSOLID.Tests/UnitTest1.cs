﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CollectionsSOLID;
using System.Threading.Tasks;

namespace CollectionsSOLID.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var types = Utils.LoadTypesFromAssembly(@"C:\dev\collections\Collections\CollectionsSOLID\bin\Debug\CollectionsSOLID.dll");

            

        }
    }
}
