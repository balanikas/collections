
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CollectionsSOLID.Tests
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void MethodExecution()
        {
            var me = new MethodExecution();

            var expectedArgValues = new List<object> {1, 2, 3};
            string expectedErrorMessage = "errormessage";
            TimeSpan expectedExecutionTime = new TimeSpan(DateTime.UtcNow.Ticks);
            string expectedName = "name";
            object expectedReturnValue = new object();
            bool expectedSuccess = true;


            me.ArgsValues = expectedArgValues;
            me.ErrorMessage = expectedErrorMessage;
            me.ExecutionTime = expectedExecutionTime;
            me.Name = expectedName;
            me.ReturnValue = expectedReturnValue;
            me.Success = expectedSuccess;
            
            Assert.AreEqual(expectedArgValues, me.ArgsValues);
            Assert.AreEqual(expectedErrorMessage, me.ErrorMessage);
            Assert.AreEqual(expectedExecutionTime, me.ExecutionTime);
            Assert.AreEqual(expectedName, me.Name);
            Assert.AreEqual(expectedReturnValue, me.ReturnValue);
            Assert.AreEqual(expectedSuccess, me.Success);

        }

        [TestMethod]
        public void ObjectBehavior()
        {
            var methodInfos = new List<MethodInfo>();
            methodInfos.Add(new DynamicMethod("", typeof(void), null));
            IBehavior behavior = new ObjectBehavior(typeof(int), methodInfos);

            Assert.AreEqual(behavior.GetObjectType(), typeof(int));
            behavior.Update();
        }

        [TestMethod]
        public void RunnerFactory()
        {
            var guiMock = new Mock<IGui>();
            guiMock.Setup(z => z.Update(It.IsAny<UIMessage>()));
            
            var loggerMock = new Mock<ILogger>();

            
            var methodInfos = new List<MethodInfo>();
            methodInfos.Add(new DynamicMethod("",typeof(void),null));
            IBehavior behavior = new ObjectBehavior(typeof(int), methodInfos);
            

            IRunner runner;
            
            
            runner = CollectionsSOLID.RunnerFactory.Get(ObjectType.BackgroundWorkerBased, behavior, guiMock.Object,loggerMock.Object);
            Assert.IsTrue(!String.IsNullOrEmpty(runner.Id));
            Assert.IsTrue(!String.IsNullOrEmpty(guiMock.Object.Id));
            Assert.AreEqual(runner.Id, guiMock.Object.Id);

            runner.AddUIListener(guiMock.Object);
            runner.AddUIListener(null);
            
            Assert.IsFalse(runner.IsAlive());

            runner.Start();
            Assert.IsTrue(runner.IsAlive());

            runner.GetState();
            runner.Destroy();


        }
    }
}
