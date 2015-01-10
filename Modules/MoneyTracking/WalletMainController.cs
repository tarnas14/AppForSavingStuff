namespace Modules.MoneyTracking
{
    using System;
    using System.Collections.Generic;
    using Ui;

    public class WalletMainController : Subscriber
    {
        private readonly WalletUi _walletUi;
        private readonly Wallet _wallet;

        public WalletMainController(WalletUi walletUi, Wallet wallet)
        {
            _walletUi = walletUi;
            _wallet = wallet;
        }

        public void Execute(UserCommand userCommand)
        {
            try
            {
                string command = userCommand.Params[0];
                string sourceName = userCommand.Params[1];

                switch (command)
                {
                    case "add":
                        _wallet.Add(sourceName, GetOperationInput(userCommand));
                        break;
                    case "sub":

                        _wallet.Subtract(sourceName, GetOperationInput(userCommand));
                        break;
                    case "trans":
                        string destinationName = userCommand.Params[2];

                        _wallet.Transfer(sourceName, destinationName, GetOperationInput(userCommand));
                        break;
                    case "balance":
                        var balance = _wallet.GetBalance(sourceName);

                        _walletUi.DisplayBalance(sourceName, balance);
                        break;
                    case "month":
                        string monthCommand = userCommand.Params[1];
                        sourceName = userCommand.Params[2];

                        _walletUi.DisplayBalance(sourceName, _wallet.DisplayMonthBalance(sourceName));
                        break;
                    case "source":
                        _wallet.CreateSource(sourceName);
                        break;
                }
            }
            catch (WalletException e)
            {
                _walletUi.DisplayError(e);
            }
        }

        private OperationInput GetOperationInput(UserCommand userCommand)
        {
            int howMuchIndex = 2;
            int descriptionIndex = 3;
            int tagsFromIndex = 4;

            if (userCommand.Params[0] == "trans")
            {
                howMuchIndex++;
                descriptionIndex++;
                tagsFromIndex++;
            }

            var tags = GetTags(userCommand, tagsFromIndex);

            return new OperationInput
            {
                Description = (userCommand.Params.Count <= descriptionIndex) ? string.Empty : userCommand.Params[descriptionIndex],
                Tags = tags,
                HowMuch = new Moneyz(Convert.ToDecimal(userCommand.Params[howMuchIndex]))
            };
        }

        private IList<Tag> GetTags(UserCommand userCommand, int tagsFrom)
        {
            var tags = new List<Tag>();
            if (userCommand.Params.Count >= tagsFrom)
            {
                for (int i = tagsFrom; i < userCommand.Params.Count; ++i)
                {
                    tags.Add(new Tag(userCommand.Params[i]));
                }
            }

            return tags;
        }
    }
}