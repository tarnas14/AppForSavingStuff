namespace Modules.MoneyTracking
{
    using System.Linq;
    using CommandHandlers;
    using Persistence;
    using Presentation;
    using Raven.Client.Linq;

    public class RemoveSourceCommandHandler : CommandHandler<RemoveSourceCommand>
    {
        private readonly BagOfRavenMagic _ravenMagic;
        private readonly WalletUi _walletUi;

        public RemoveSourceCommandHandler(BagOfRavenMagic ravenMagic, WalletUi walletUi)
        {
            _ravenMagic = ravenMagic;
            _walletUi = walletUi;
        }

        public void Handle(RemoveSourceCommand command)
        {
            RemoveSource(command.Source);

            _walletUi.DisplayInformation(string.Format("{0} removed", command.Source));
        }

        private void RemoveSource(string source)
        {
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var operations =
                    _ravenMagic.WaitForQueryIfNecessary(session.Query<Operations_BySources.Result, Operations_BySources>())
                        .Where(operation => operation.SourceName == source)
                        .OfType<Operation>()
                        .ToList();

                operations.ForEach(session.Delete);
                session.SaveChanges();
            }
        }
    }
}