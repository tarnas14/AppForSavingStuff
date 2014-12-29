namespace Specification.WalletSpec
{
    using System;
    using System.Runtime.Remoting;
    using Modules;
    using Modules.MoneyTracking;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class OperationsFactorySpec
    {
        private Mock<TimeMaster> _timeMasterMock;
        private OperationFactory _factory;

        [SetUp]
        public void Setup()
        {
            _timeMasterMock = new Mock<TimeMaster>();
            _factory = new OperationFactory(_timeMasterMock.Object);
        }

        [Test]
        public void ShouldReturnOperationForIn()
        {
            //given
            var now = DateTime.Now;
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(now);
            var howMuch = new Moneyz(5);
            const string sourceName = "testSourceName";

            //when
            var inOperation = _factory.GetInOperation(sourceName, howMuch);

            //then
            Assert.That(inOperation.Type, Is.EqualTo(OperationType.In));
            Assert.That(inOperation.When, Is.EqualTo(now));
            Assert.That(inOperation.Source, Is.EqualTo(sourceName));
            Assert.That(inOperation.HowMuch, Is.EqualTo(howMuch));
            Assert.That(string.IsNullOrEmpty(inOperation.Destination));
        }

        [Test]
        public void ShouldCreateOutOperation()
        {
            //given
            var now = DateTime.Now;
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(now);
            var howMuch = new Moneyz(5);
            const string sourceName = "testSourceName";

            //when
            var inOperation = _factory.GetOutOperation(sourceName, howMuch);

            //then
            Assert.That(inOperation.Type, Is.EqualTo(OperationType.Out));
            Assert.That(inOperation.When, Is.EqualTo(now));
            Assert.That(inOperation.Source, Is.EqualTo(sourceName));
            Assert.That(inOperation.HowMuch, Is.EqualTo(howMuch));
            Assert.That(string.IsNullOrEmpty(inOperation.Destination));
        }

        [Test]
        public void ShouldCreateTransferOperation()
        {
            //given
            var now = DateTime.Now;
            _timeMasterMock.SetupGet(mock => mock.Now).Returns(now);
            var howMuch = new Moneyz(5);
            const string sourceName = "testSourceName";
            const string destinationName = "testDestinationName";

            //when
            var inOperation = _factory.GetTransferOperation(sourceName, destinationName, howMuch);

            //then
            Assert.That(inOperation.Type, Is.EqualTo(OperationType.Transfer));
            Assert.That(inOperation.When, Is.EqualTo(now));
            Assert.That(inOperation.Source, Is.EqualTo(sourceName));
            Assert.That(inOperation.Destination, Is.EqualTo(destinationName));
            Assert.That(inOperation.HowMuch, Is.EqualTo(howMuch));
        }
    }
}
