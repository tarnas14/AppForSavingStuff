﻿namespace Modules.MoneyTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommandHandlers;
    using Presentation;
    using Tarnas.ConsoleUi;

    public class WalletMainController : Subscriber
    {
        private readonly WalletUi _walletUi;
        private readonly WalletHistory _ravenHistory;
        private readonly TimeMaster _timeMaster;
        private ReservedWordsStore _reservedWordsStore;

        public WalletMainController(WalletUi walletUi, WalletHistory ravenHistory, TimeMaster timeMaster, ReservedWordsStore reservedWordsStore)
        {
            _walletUi = walletUi;
            _ravenHistory = ravenHistory;
            _timeMaster = timeMaster;
            _reservedWordsStore = reservedWordsStore;
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
                            Tags = GetTags(userCommand, 4),
                            When = GetDate(userCommand)
                        };
                        new OperationCommandHandler(_ravenHistory, _timeMaster, _reservedWordsStore).Execute(addCommand);
                        break;
                    case "sub":
                        var subCommand = new OperationCommand
                        {
                            Source = userCommand.Params[1],
                            Description = (userCommand.Params.Count <= 3) ? string.Empty : userCommand.Params[3],
                            HowMuch = new Moneyz(-Convert.ToDecimal(userCommand.Params[2])),
                            Tags = GetTags(userCommand, 4),
                            When = GetDate(userCommand)
                        };
                        new OperationCommandHandler(_ravenHistory, _timeMaster, _reservedWordsStore).Execute(subCommand);

                        break;
                    case "trans":
                        var transCommand = new OperationCommand
                        {
                            Source = userCommand.Params[1],
                            Destination = userCommand.Params[2],
                            Description = (userCommand.Params.Count <= 4) ? string.Empty : userCommand.Params[4],
                            HowMuch = new Moneyz(Convert.ToDecimal(userCommand.Params[3])),
                            Tags = GetTags(userCommand, 5),
                            When = GetDate(userCommand)
                        };;
                        new OperationCommandHandler(_ravenHistory, _timeMaster, _reservedWordsStore).Execute(transCommand);

                        break;
                    case "balance":
                        var displayBalanceCommand = new DisplayBalanceCommand
                        {
                            Sources = GetParamsFrom(1, userCommand.Params),
                            Month = GetMonth(userCommand)
                        };
                        new DisplayBalanceCommandHandler(_ravenHistory, _walletUi).Execute(displayBalanceCommand);

                        break;
                    case "history":
                        var displayHistoryCommand = new DisplayHistoryCommand
                        {
                            Monthly = userCommand.Flags.Contains("m"),
                            DisplayTags = userCommand.Flags.Contains("t"),
                            DisplayDescriptions = userCommand.Flags.Contains("d")
                        };

                        displayHistoryCommand.Sources = GetParamsFrom(1, userCommand.Params);

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

        private Month GetMonth(UserCommand userCommand)
        {
            if (!userCommand.Flags.Contains("m"))
            {
                return null;
            }

            string monthParam;
            if (!userCommand.TryGetParam("month", out monthParam))
            {
                return new Month(_timeMaster.Today.Year, _timeMaster.Today.Month);
            }

            return Month.FromString(monthParam);
        }

        private DateTime GetDate(UserCommand userCommand)
        {
            string dateString = string.Empty;
            if (userCommand.TryGetParam("date", out dateString))
            {
                return Convert.ToDateTime(dateString);
            }

            return _timeMaster.Today;
        }

        private IList<string> GetParamsFrom(int startIndex, IList<string> parameters)
        {
            if (parameters.Count <= startIndex)
            {
                return new List<string>();
            }

            return Enumerable.Range(startIndex, parameters.Count - startIndex).Select(index => parameters[index]).ToList();
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