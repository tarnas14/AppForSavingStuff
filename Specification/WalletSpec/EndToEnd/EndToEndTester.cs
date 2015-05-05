﻿namespace Specification.WalletSpec.EndToEnd
{
    using System;
    using System.Linq;
    using Halp;
    using Modules;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using Modules.MoneyTracking.Presentation;
    using Modules.MoneyTracking.SourceNameValidation;
    using Moq;
    using NUnit.Framework;
    using Tarnas.ConsoleUi;

    class EndToEndTester
    {
        private readonly ConsoleUi _ui;
        private readonly ConsoleMock _consoleMock;
        private readonly Mock<TimeMaster> _timeMasterMock;
        private readonly MemoryListSourceNameValidator _sourceNameValidator;
        private DateTime _date;

        public EndToEndTester()
        {
            _ui = new ConsoleUi();
            _consoleMock = new ConsoleMock();

            var ravenHistory = new RavenDocumentStoreWalletHistory(
                new DocumentStoreProvider
                {
                    RunInMemory = true
                }
            )
            {
                WaitForNonStale = true
            };

            _timeMasterMock = new Mock<TimeMaster>();
            _timeMasterMock.Setup(master => master.Now).Returns(() => DateTime.Now);
            _timeMasterMock.Setup(master => master.Today).Returns(() => DateTime.Now);
            _sourceNameValidator = new MemoryListSourceNameValidator();

            _ui.Subscribe(new WalletMainController(new WalletUi(_consoleMock), ravenHistory, _timeMasterMock.Object, _sourceNameValidator), "wallet");
        }

        public EndToEndTester Execute(string userCommandString)
        {
            _ui.UserInput(userCommandString);

            return this;
        }

        public EndToEndTester Execute(params string[] userCommands)
        {
            userCommands.ToList().ForEach(userCommand => Execute(userCommand));

            return this;
        }

        public EndToEndTester SetTime(DateTime date)
        {
            _timeMasterMock.Setup(master => master.Now).Returns(() =>
            {
                var now = DateTime.Now;
                return new DateTime(date.Year, date.Month, date.Day, now.Hour, now.Minute, now.Second,
                    now.Millisecond);
            });
            _timeMasterMock.Setup(master => master.Today).Returns(() =>
            {
                var now = DateTime.Now;
                return new DateTime(date.Year, date.Month, date.Day, now.Hour, now.Minute, now.Second,
                    now.Millisecond);
            });

            return this;
        }

        public void AssertExpectedResult(params string[] expectedOutput)
        {
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        public void ReserveWord(string wordToReserve)
        {
            _sourceNameValidator.RestrictWord(wordToReserve);
        }
    }
}
