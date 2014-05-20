using Collections.Runtime;
using Moq;
using NUnit.Framework;

namespace Collections.Tests
{
    [TestFixture]
    class RunnerCollectionTests
    {

        [Test]
        public void RunnerCollection_Basic()
        {
            var rc = new RunnerCollection();

            var mockedRunner = new Mock<IRunner>();
            mockedRunner.SetupGet(x => x.Id).Returns("id");
            mockedRunner.Setup(x => x.IsAlive()).Returns(true);


            rc.Add(mockedRunner.Object);
            var runner = rc.GetById(mockedRunner.Object.Id);
            Assert.IsNotNull(runner);


            rc.Add(mockedRunner.Object);
            CollectionAssert.IsNotEmpty(rc.GetActiveRunners());
            rc.RemoveById(mockedRunner.Object.Id);
            CollectionAssert.IsEmpty(rc.GetActiveRunners());

            rc.Add(mockedRunner.Object);
            CollectionAssert.IsNotEmpty(rc.GetActiveRunners());
            rc.Remove(mockedRunner.Object);
            CollectionAssert.IsEmpty(rc.GetActiveRunners());

            rc.Add(mockedRunner.Object);
            CollectionAssert.IsNotEmpty(rc.GetActiveRunners());
            rc.RemoveAll();
            CollectionAssert.IsEmpty(rc.GetActiveRunners());
            
        }
    }
}
