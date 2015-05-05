namespace Modules.MoneyTracking
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommandHandlers;
    using Presentation;
    using SourceNameValidation;
    using Tarnas.ConsoleUi;

    public class WalletMainController : Subscriber
    {
        private readonly WalletUi _walletUi;
        private readonly WalletHistory _ravenHistory;
        private readonly TimeMaster _timeMaster;
        private readonly SourceNameValidator _sourceNameValidator;

        public WalletMainController(WalletUi walletUi, WalletHistory ravenHistory, TimeMaster timeMaster, SourceNameValidator sourceNameValidator)
        {
            _walletUi = walletUi;
            _ravenHistory = ravenHistory;
            _timeMaster = timeMaster;
            _sourceNameValidator = sourceNameValidator;
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
                        new OperationCommandHandler(_ravenHistory, _timeMaster, _sourceNameValidator).Execute(addCommand);
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
                        new OperationCommandHandler(_ravenHistory, _timeMaster, _sourceNameValidator).Execute(subCommand);

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
                        new OperationCommandHandler(_ravenHistory, _timeMaster, _sourceNameValidator).Execute(transCommand);

                        break;
                    case "balance":
                        var displayBalanceCommand = new DisplayBalanceCommand
                        {
                            Sources = GetParamsFrom(1, userCommand.Params),
                            Month = GetMonthForBalanceDisplay(userCommand)
                        };

                        new DisplayBalanceCommandHandler(_ravenHistory, _walletUi).Execute(displayBalanceCommand);

                        break;
                    case "history":
                        var displayHistoryCommand = new DisplayHistoryCommand
                        {
                            Monthly = userCommand.Flags.Contains("m"),
                            Month = GetMonthForHistoryDisplay(userCommand),
                            DisplayTags = userCommand.Flags.Contains("t"),
                            DisplayDescriptions = userCommand.Flags.Contains("d")
                        };

                        displayHistoryCommand.Sources = GetParamsFrom(1, userCommand.Params);

                        new DisplayHistoryCommandHandler(_ravenHistory, _walletUi, _timeMaster).Execute(displayHistoryCommand);

                        break;
                    case "tags":
                        new DisplayTagsCommandHandler(_ravenHistory, _walletUi).Execute(new DisplayTagsCommand());
                        break;

                    case "remove":
                        var removeSourceCommand = new RemoveSourceCommand
                        {
                            Source = userCommand.Params[1]
                        };

                        new RemoveSourceCommandHandler(_ravenHistory, _walletUi).Execute(removeSourceCommand);
                        break;
                }
            }
            catch (WalletException e)
            {
                _walletUi.DisplayError(e);
            }
        }

        private Month GetMonthForHistoryDisplay(UserCommand userCommand)
        {
            string month;
            if (userCommand.TryGetParam("month", out month))
            {
                return Month.FromString(month);
            }

            if (userCommand.Flags.Contains("m"))
            {
                return Month.FromToday(_timeMaster);
            }

            return null;
        }

        private Month GetMonthForBalanceDisplay(UserCommand userCommand)
        {
            if (!userCommand.Flags.Contains("m"))
            {
                return null;
            }

            string monthParam;
            if (!userCommand.TryGetParam("month", out monthParam))
            {
                return Month.FromToday(_timeMaster);
            }

            return Month.FromString(monthParam);
        }

        private DateTime GetDate(UserCommand userCommand)
        {
            string dateString = string.Empty;
            if (userCommand.TryGetParam("date", out dateString))
            {
                var dayFromString = Convert.ToDateTime(dateString);
                return new DateTime(dayFromString.Year, dayFromString.Month, dayFromString.Day, _timeMaster.Now.Hour, _timeMaster.Now.Minute, _timeMaster.Now.Second, _timeMaster.Now.Millisecond);
            }

            return _timeMaster.Now;
        }

        private IList<string> GetParamsFrom(int startIndex, IList<string> parameters)
        {
            if (parameters.Count <= startIndex)
            {
                return new List<string>();
            }
            return parameters.Skip(startIndex).ToList();
        }

        private IList<Tag> GetTags(UserCommand userCommand, int tagsFrom)
        {
            var paramsThatCouldBeTags = userCommand.Params.Skip(tagsFrom);

            if (!paramsThatCouldBeTags.Any())
            {
                return new Tag[] {};
            }

            var tags = paramsThatCouldBeTags.Where(Tag.IsTagName).Select(param => new Tag(param));
            
            return tags.ToList();
        }
    }
}