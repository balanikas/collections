using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Collections.Compiler;
using Collections.Messages;
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
        private BroadcastBlock<CompilerServiceOutputMessage> _outputBroadcasts;


        [SetUp]
        public void SetUp()
        {
            _consumableBroadcasts = new BroadcastBlock<CompilerServiceMessage>(null);
            _outputBroadcasts = new BroadcastBlock<CompilerServiceOutputMessage>(null);
            _service = new CompilerService(_consumableBroadcasts, _outputBroadcasts);
        }

        [Test]
        public void CompilerService_StartAndStop_VerifyActionIsExecuted()
        {
           

            var actionCount = 0;

            var action = new Func<CompilerServiceMessage, CompilerServiceOutputMessage>(message =>
            {
                Assert.AreEqual(message.Source, _messageToConsume.Source);
                actionCount++;
                return new CompilerServiceOutputMessage(new List<string>(), new List<LoadedType>(),ServiceMessageState.Succeeded);
            });

            _service.Start(action, TimeSpan.FromMilliseconds(100));

            _messageToConsume = new CompilerServiceMessage("source", ServiceMessageState.Succeeded);


            for (int i = 0; i < 1000; i++)
            {
                var message = new CompilerServiceMessage("source", ServiceMessageState.Succeeded);
                Thread.Sleep(1);
                _consumableBroadcasts.Post(message);
                Thread.Sleep(1);
                var serviceMessage = _outputBroadcasts.Receive();
                Assert.AreEqual(ServiceMessageState.Succeeded,serviceMessage.State);
            }
            
            Assert.IsTrue(actionCount > 0);
            
            _service.Stop();
            Thread.Sleep(1000);
        }

        [Test]
        public void CompilerService_StartAndStop()
        {


            var action = new Func<CompilerServiceMessage, CompilerServiceOutputMessage>(message =>
            {
                return new CompilerServiceOutputMessage(new List<string>(), new List<LoadedType>(), ServiceMessageState.Succeeded);
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



            var action = new Func<CompilerServiceMessage, CompilerServiceOutputMessage>(message =>
            {
                return new CompilerServiceOutputMessage(new List<string>(), new List<LoadedType>(), ServiceMessageState.Succeeded);
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

            var action = new Func<CompilerServiceMessage, CompilerServiceOutputMessage>(message =>
            {
                return new CompilerServiceOutputMessage(new List<string>(), new List<LoadedType>(), ServiceMessageState.Succeeded);
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
