namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;
    using Persistence;
    using Presentation;

    public class DisplayTagsCommandHandler : CommandHandler<DisplayTagsCommand>
    {
        private readonly BagOfRavenMagic _ravenMagic;
        private readonly WalletUi _walletUi;

        public DisplayTagsCommandHandler(BagOfRavenMagic ravenMagic, WalletUi walletUi)
        {
            _ravenMagic = ravenMagic;
            _walletUi = walletUi;
        }

        public void Handle(DisplayTagsCommand command)
        {
            var tags = GetAllTags();
            _walletUi.DisplayTags(tags);
        }

        private IEnumerable<Tag> GetAllTags()
        {
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var tags = _ravenMagic.WaitForQueryIfNecessary(session.Query<Tag>());

                return tags;
            }
        }
    }
}