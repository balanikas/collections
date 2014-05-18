using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Collections.Compiler;
using Collections.Runtime;
using NUnit.Framework;

namespace Collections.Tests
{
    [TestFixture]
    public class CompilerServiceTests
    {
        private CompilerServiceMessage _messageToConsume;
        [Test]
        public void CompilerService_StartAndStop_VerifyActionIsExecuted()
        {
            var consumableBroadcasts = new BroadcastBlock<CompilerServiceMessage>(null);
            var service = new CompilerService(consumableBroadcasts);

            var actionCount = 0;

            var action = new Action<CompilerServiceMessage>(message =>
            {
                Assert.AreEqual(message.Source, _messageToConsume.Source);
                message.State = ServiceMessageState.Succeeded;
                actionCount++;
            });

            service.Start(action, TimeSpan.FromMilliseconds(100));

            _messageToConsume = new CompilerServiceMessage("source", ServiceMessageState.Succeeded);


            for (int i = 0; i < 1000; i++)
            {
                var message = new CompilerServiceMessage("source", ServiceMessageState.Succeeded);
                Thread.Sleep(1);
                consumableBroadcasts.Post(message);
                Thread.Sleep(1);
                var serviceMessage = consumableBroadcasts.Receive();
                Assert.AreEqual(ServiceMessageState.Succeeded,serviceMessage.State);
            }
            
            Assert.IsTrue(actionCount > 0);
            
            service.Stop();
            Thread.Sleep(1000);
        }

        [Test]
        public void CompilerService_StartAndStop()
        {

            var consumableBroadcasts = new BroadcastBlock<CompilerServiceMessage>(null);
            var service = new CompilerService(consumableBroadcasts);


            var action = new Action<CompilerServiceMessage>(message =>
            {
            });


            Assert.DoesNotThrow(() =>
            {
                service.Start(action, TimeSpan.FromMilliseconds(100));
                Assert.IsTrue(service.IsRunning);
                service.Stop();
                Assert.IsFalse(service.IsRunning);

            });

            
        }

        [Test]
        public void CompilerService_StartAndStop_MultipleTimes()
        {

            var consumableBroadcasts = new BroadcastBlock<CompilerServiceMessage>(null);
            var service = new CompilerService(consumableBroadcasts);


            var action = new Action<CompilerServiceMessage>(message =>
            {
            });


            Assert.DoesNotThrow(() =>
            {
                service.Start(action, TimeSpan.FromMilliseconds(100));
                service.Start(action, TimeSpan.FromMilliseconds(100));
                Assert.IsTrue(service.IsRunning);

                service.Stop();
                service.Stop();
                Assert.IsFalse(service.IsRunning);
            });


        }

        [Test]
        public void CompilerService_ChangeExecutionInterval()
        {
            var consumableBroadcasts = new BroadcastBlock<CompilerServiceMessage>(null);
            var service = new CompilerService(consumableBroadcasts);


            var action = new Action<CompilerServiceMessage>(message =>
            {
               
            });
            service.Start(action, TimeSpan.FromMilliseconds(100));
            Assert.DoesNotThrow(() =>
            {
                service.ExecutionInterval = TimeSpan.FromMilliseconds(10);
                service.ExecutionInterval = TimeSpan.FromMilliseconds(1000);
                service.ExecutionInterval = TimeSpan.FromMilliseconds(10);
                service.ExecutionInterval = TimeSpan.FromMilliseconds(1);
                Thread.Sleep(2000);
            });

            


        }


    }
}
