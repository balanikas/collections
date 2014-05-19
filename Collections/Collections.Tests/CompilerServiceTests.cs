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
        private CompilerService _service;
        private BroadcastBlock<CompilerServiceMessage> _consumableBroadcasts;


        [SetUp]
        public void SetUp()
        {
            _consumableBroadcasts = new BroadcastBlock<CompilerServiceMessage>(null);
            _service = new CompilerService(_consumableBroadcasts);
        }

        [Test]
        public void CompilerService_StartAndStop_VerifyActionIsExecuted()
        {
           

            var actionCount = 0;

            var action = new Action<CompilerServiceMessage>(message =>
            {
                Assert.AreEqual(message.Source, _messageToConsume.Source);
                message.State = ServiceMessageState.Succeeded;
                actionCount++;
            });

            _service.Start(action, TimeSpan.FromMilliseconds(100));

            _messageToConsume = new CompilerServiceMessage("source", ServiceMessageState.Succeeded);


            for (int i = 0; i < 1000; i++)
            {
                var message = new CompilerServiceMessage("source", ServiceMessageState.Succeeded);
                Thread.Sleep(1);
                _consumableBroadcasts.Post(message);
                Thread.Sleep(1);
                var serviceMessage = _consumableBroadcasts.Receive();
                Assert.AreEqual(ServiceMessageState.Succeeded,serviceMessage.State);
            }
            
            Assert.IsTrue(actionCount > 0);
            
            _service.Stop();
            Thread.Sleep(1000);
        }

        [Test]
        public void CompilerService_StartAndStop()
        {


            var action = new Action<CompilerServiceMessage>(message =>
            {
            });


            Assert.DoesNotThrow(() =>
            {
                _service.Start(action, TimeSpan.FromMilliseconds(100));
                Assert.IsTrue(_service.IsRunning);
                _service.Stop();
                Assert.IsFalse(_service.IsRunning);

            });

            
        }

        [Test]
        public void CompilerService_StartAndStop_MultipleTimes()
        {

           

            var action = new Action<CompilerServiceMessage>(message =>
            {
            });


            Assert.DoesNotThrow(() =>
            {
                _service.Start(action, TimeSpan.FromMilliseconds(100));
                _service.Start(action, TimeSpan.FromMilliseconds(100));
                Assert.IsTrue(_service.IsRunning);

                _service.Stop();
                _service.Stop();
                Assert.IsFalse(_service.IsRunning);
            });


        }

        [Test]
        public void CompilerService_ChangeExecutionInterval()
        {
            
            var action = new Action<CompilerServiceMessage>(message =>
            {
               
            });
            _service.Start(action, TimeSpan.FromMilliseconds(100));
            Assert.DoesNotThrow(() =>
            {
                _service.ExecutionInterval = TimeSpan.FromMilliseconds(10);
                _service.ExecutionInterval = TimeSpan.FromMilliseconds(1000);
                _service.ExecutionInterval = TimeSpan.FromMilliseconds(10);
                _service.ExecutionInterval = TimeSpan.FromMilliseconds(1);
                Thread.Sleep(2000);
            });

            


        }


    }
}
