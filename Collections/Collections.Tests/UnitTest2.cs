using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Collections.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Collections.Tests
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void MethodExecution()
        {
            var me = new MethodExecutionResult();

            var expectedArgValues = new List<object> {1, 2, 3};
            string expectedErrorMessage = "errormessage";
            var expectedExecutionTime = new TimeSpan(DateTime.UtcNow.Ticks);
            string expectedName = "name";
            var expectedReturnValue = new object();
            bool expectedSuccess = true;


            // me.ArgsValues = expectedArgValues;
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
            methodInfos.Add(new DynamicMethod("", typeof (void), null));
            IRunnable runnable = new RunnableObject(typeof (int), methodInfos);

            Assert.AreEqual(runnable.GetObjectType(), typeof (int));
            //runnable.Update();
        }

        [TestMethod]
        public void RunnerFactory()
        {
            //var guiMock = new Mock<IGui>();
            //guiMock.Setup(z => z.Update(It.IsAny<MethodExecutionMessage>()));

            //var loggerMock = new Mock<ILogger>();


            //var methodInfos = new List<MethodInfo>();
            //methodInfos.Add(new DynamicMethod("", typeof (void), null));
            //IRunnable runnable = new RunnableObject(typeof (int), methodInfos);


            //IRunner runner;

            //var settings = new RunnerSettings();
            //settings.Iterations = 100;
            //settings.RunnerType = RunnerType.BackgroundWorkerBased;
            
            //runner = Collections.RunnerFactory.Get( runnable, guiMock.Object,
            //    loggerMock.Object,settings );
            //Assert.IsTrue(!String.IsNullOrEmpty(runner.Id));
            //Assert.IsTrue(!String.IsNullOrEmpty(guiMock.Object.Id));
            //Assert.AreEqual(runner.Id, guiMock.Object.Id);

            //runner.AddUiListener(guiMock.Object);
            //runner.AddUiListener(null);

            //Assert.IsFalse(runner.IsAlive());

            //runner.Start();
            //Assert.IsTrue(runner.IsAlive());

            //runner.GetCurrentState();
            //runner.Destroy();
        }
    }
}