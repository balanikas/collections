using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Collections.Runtime;
using NUnit.Framework;

namespace Collections.Tests
{
    [TestFixture]
    class RunnableItemTests
    {
        [Test]
        public void RunnableItem_SimpleUpdate()
        {
            var type = typeof (int);

            var methods = type.GetMethods().Where(x => x.Name == "ToString" && !x.GetParameters().Any()).ToList();
            var runnable = new RunnableItem(type,methods);

            Assert.AreEqual(type,runnable.ObjectType);
            runnable.Update(false);
        }
    }
}
