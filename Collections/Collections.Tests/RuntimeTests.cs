﻿using Collections.Logging;
using Collections.Runtime;
using Moq;
using NUnit.Framework;

namespace Collections.Tests
{
    [TestFixture]
    class RuntimeTests
    {
        [Test]
        public void Runtime_StartAndStop()
        {

            var mockedLogger = new Mock<ILogger>();
            var runtime = new Runtime.Runtime(mockedLogger.Object);

            Assert.IsFalse(runtime.IsRunning());
            Assert.IsNotNull(runtime.Logger);
            runtime.Start();
            Assert.IsTrue(runtime.IsRunning());
            runtime.Stop();
            Assert.IsFalse(runtime.IsRunning());
            runtime.Reset();
            Assert.IsTrue(runtime.IsRunning());

        }

        [Test]
        public void Runtime_CreateAndAddRunner()
        {
            var mockedLogger = new Mock<ILogger>();
            var runtime = new Runtime.Runtime(mockedLogger.Object);

            var mockedRunnable = new Mock<IRunnable>();
            var settings = new RunnerSettings();

            var runner = runtime.CreateAndAddRunner(mockedRunnable.Object, settings);
        
            Assert.IsNotNull(runner);

        
        }

    }
}
