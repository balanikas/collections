using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Collections.Compiler;
using Collections.Logging;
using Collections.Messages;
using Collections.Runtime;
using Moq;
using NUnit.Framework;

namespace Collections.Tests
{
    [TestFixture]
    class BWBasedRunnerTests
    {
        [Test]
        public void BWBasedRunner_StartAndStop()
        {
            var type = typeof(int);

            var methods = type.GetMethods().Where(x => x.Name == "ToString" && !x.GetParameters().Any()).ToList();
            var runnable = new RunnableItem(type, methods);

            var mockedLogger = new Mock<ILogger>();

            var settings = new RunnerSettings()
            {
                CompilerServiceType = CompilerType.Default,
                Iterations = 10,
                RunnerType = RunnerType.BackgroundWorkerBased
            };

            var runner = new BWBasedRunner(runnable, mockedLogger.Object, settings);

            runner.Start();
            runner.Start();
            Assert.IsTrue(runner.IsAlive());
            Assert.IsNotNull(runner.GetCurrentState());
            runner.Destroy();
            Thread.Sleep(1000);
            Assert.IsFalse(runner.IsAlive());
            Assert.IsNotNull(runner.GetCurrentState());
            
            

        }

        [Test]
        public void BWBasedRunner_AddAndRemoveListeners()
        {
            var type = typeof(int);

            var methods = type.GetMethods().Where(x => x.Name == "ToString" && !x.GetParameters().Any()).ToList();
            var runnable = new RunnableItem(type, methods);

            var mockedLogger = new Mock<ILogger>();

            var settings = new RunnerSettings()
            {
                CompilerServiceType = CompilerType.Default,
                Iterations = 10,
                RunnerType = RunnerType.BackgroundWorkerBased
            };

            var runner = new BWBasedRunner(runnable, mockedLogger.Object, settings);
            var listener = new UiListener();

            Assert.DoesNotThrow(() =>
            {
                runner.AddUiListener(listener);
                runner.AddUiListener(listener);
                runner.RemoveUiListener(listener);
                runner.RemoveUiListener(listener);
            });

            Assert.AreEqual(runner.Id, listener.Id);
            runner.Destroy();
            Thread.Sleep(1000);

        }
    }

    class UiListener : IGui
    {
        public string Id { get; set; }
        public void Initialize()
        {
        }

        public void Update(MethodExecutionMessage message)
        {
        }

        public void Destroy()
        {
           
        }
    }
}
