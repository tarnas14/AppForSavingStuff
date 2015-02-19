namespace Modules.MoneyTracking
{
    using System;
    using System.Collections.Generic;
    using CommandHandlers;
    using Presentation;
    using Tarnas.ConsoleUi;

    public class WalletMainController : Subscriber
    {
        private readonly WalletUi _walletUi;
        private readonly Wallet _wallet;
        private readonly WalletHistory _ravenHistory;
        private readonly TimeMaster _timeMaster;

        public WalletMainController(WalletUi walletUi, Wallet wallet, WalletHistory ravenHistory, TimeMaster timeMaster)
        {
            _walletUi = walletUi;
            _wallet = wallet;
            _ravenHistory = ravenHistory;
            _timeMaster = timeMaster;
        }

        public void Execute(UserCommand userCommand)
        {
            try
            {
                string command = userCommand.Params[0];
                string sourceName = string.Empty;

                switch (command)
                {
                    case "add":
                        sourceName = userCommand.Params[1];

                        var addCommand = new AddCommand
                        {
                            Source = sourceName,
                            OperationInput = GetOperationInput(userCommand)
                        };
                        new AddCommandHandler(_ravenHistory, _timeMaster).Execute(addCommand);
                        break;
                    case "sub":
                        sourceName = userCommand.Params[1];

                        var subCommand = new SubtractCommand
                        {
                            Source = sourceName,
                            OperationInput = GetOperationInput(userCommand)
                        };
                        new SubtractCommandHandler(_ravenHistory, _timeMaster).Execute(subCommand);

                        break;
                    case "trans":
                        sourceName = userCommand.Params[1];
                        string destinationName = userCommand.Params[2];

                        var transCommand = new TransferCommand
                        {
                            Source = sourceName,
                            Destination = destinationName,
                            OperationInput = GetOperationInput(userCommand)
                        };
                        new TransferCommandHandler(_ravenHistory, _timeMaster).Execute(transCommand);

                        break;
                    case "balance":
                        sourceName = userCommand.Params[1];

                        var displayBalanceCommand = new DisplayBalanceCommand
                        {
                            Sources = new[] { sourceName }
                        };
                        new DisplayBalanceCommandHandler(_ravenHistory, _walletUi).Execute(displayBalanceCommand);

                        break;
                    case "month":
                        sourceName = userCommand.Params[1];

                        if (userCommand.Flags.Contains("t"))
                        {
                            DisplayBalanceForTag(userCommand.Params[1]);
                        }
                        else
                        {
                            _walletUi.DisplayBalance(sourceName, _wallet.DisplayMonthBalance(sourceName));
                        }

                        break;
                    case "source":
                        sourceName = userCommand.Params[1];
                        var newSourceCommand = new CreateSourceCommand
                        {
                            Name = sourceName
                        };
                        new CreateSourceCommandHandler(_ravenHistory, new HardcodedReservedWordsStore()).Execute(newSourceCommand);
                        break;
                    case "history":
                        var displayHistoryCommand = new DisplayHistoryCommand();

                        new DisplayHistoryCommandHandler(_ravenHistory, _walletUi).Execute(displayHistoryCommand);

                        break;

                        var verbosity = new HistoryDisplayVerbosity
                        {
                            Tags = userCommand.Flags.Contains("t")
                        };

                        var filters = new HistoryDisplayFilter
                        {
                            Source = userCommand.Params.Count == 2 ? userCommand.Params[1] : string.Empty
                        };

                        if (userCommand.Flags.Contains("m"))
                        {
                            _walletUi.DisplayHistory(_wallet.GetHistoryForThisMonth(filters), verbosity);
                        }
                        else
                        {
                            _walletUi.DisplayHistory(_wallet.GetFullHistory(), verbosity);
                        }

                        break;
                    case "tags":
                        var tagsUsedThisMonth = _wallet.GetTagsUsedThisMonth();
                        _walletUi.DisplayTags(tagsUsedThisMonth);
                        break;
                }
            }
            catch (WalletException e)
            {
                _walletUi.DisplayError(e);
            }
        }

        private void DisplayBalanceForSource(string sourceName)
        {
            var balance = _wallet.GetBalance(sourceName);

            _walletUi.DisplayBalance(sourceName, balance);
        }

        private void DisplayBalanceForTag(string tag)
        {
            var history = _wallet.GetTagHistoryForThisMonth(tag);

            _walletUi.DisplayHistory(history);
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