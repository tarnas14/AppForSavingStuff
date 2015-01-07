namespace Specification.WalletSpec
{
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    class WalletWithRavenRepoSpec : WalletSpec
    {
        [SetUp]
        public new void Setup()
        {
            TimeMasterMock = new Mock<TimeMaster>();
            var inMemoryStoreProvider = new DocumentStoreProvider
            {
                RunInMemory = true
            };
            Wallet = new Wallet(new RavenDocumentStoreWalletHistory(inMemoryStoreProvider){WaitForNonStale = true}, TimeMasterMock.Object);
        }

    }
}
