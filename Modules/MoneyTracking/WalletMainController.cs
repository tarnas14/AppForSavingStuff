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
                        var addCommand = new OperationCommand
                        {
                            Source = userCommand.Params[1],
                            Description = (userCommand.Params.Count <= 3) ? string.Empty : userCommand.Params[3],
                            HowMuch = new Moneyz(Convert.ToDecimal(userCommand.Params[2])),
                            Tags = GetTags(userCommand, 4)
                        };
                        new OperationCommandHandler(_ravenHistory, _timeMaster).Execute(addCommand);
                        break;
                    case "sub":
                        var subCommand = new OperationCommand
                        {
                            Source = userCommand.Params[1],
                            Description = (userCommand.Params.Count <= 3) ? string.Empty : userCommand.Params[3],
                            HowMuch = new Moneyz(-Convert.ToDecimal(userCommand.Params[2])),
                            Tags = GetTags(userCommand, 4)
                        };
                        new OperationCommandHandler(_ravenHistory, _timeMaster).Execute(subCommand);

                        break;
                    case "trans":
                        var transCommand = new OperationCommand
                        {
                            Source = userCommand.Params[1],
                            Destination = userCommand.Params[2],
                            Description = (userCommand.Params.Count <= 4) ? string.Empty : userCommand.Params[4],
                            HowMuch = new Moneyz(Convert.ToDecimal(userCommand.Params[3])),
                            Tags = GetTags(userCommand, 5)
                        };;
                        new OperationCommandHandler(_ravenHistory, _timeMaster).Execute(transCommand);

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
                        var displayHistoryCommand = new DisplayHistoryCommand
                        {
                            Monthly = userCommand.Flags.Contains("m"),
                            Verbosity = new HistoryDisplayVerbosity
                            {
                                Tags = userCommand.Flags.Contains("t")
                            }
                        };

                        if (userCommand.Params.Count == 2)
                        {
                            displayHistoryCommand.Sources = new[] {userCommand.Params[1]};
                        }

                        new DisplayHistoryCommandHandler(_ravenHistory, _walletUi, _timeMaster).Execute(displayHistoryCommand);

                        break;
                    case "tags":
                        new DisplayTagsCommandHandler(_ravenHistory, _walletUi).Execute(new DisplayTagsCommand());
                        break;
                }
            }
            catch (WalletException e)
            {
                _walletUi.DisplayError(e);
            }
        }

        private void DisplayBalanceForTag(string tag)
        {
            var history = _wallet.GetTagHistoryForThisMonth(tag);

            _walletUi.DisplayHistory(history);
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