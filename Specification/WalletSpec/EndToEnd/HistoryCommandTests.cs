namespace Specification.WalletSpec.EndToEnd
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;

    [TestFixture]
    class HistoryCommandTests : EndToEndBaseFixture
    {
        [Test]
        public void ShouldDisplayHistoryForCurrentMonth()
        {
            //given
            const string source = "sourceName";
            Ui.UserInput(string.Format("/wallet source {0}", source));

            TimeMasterMock.SetupGet(mock => mock.Now).Returns(new DateTime(2000, 11, 30));
            Ui.UserInput(string.Format("/wallet add {0} 4", source));

            var today = new DateTime(2000, 12, 1);
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(today);
            Ui.UserInput(string.Format("/wallet add {0} 2", source));

            TimeMasterMock.SetupGet(mock => mock.Today).Returns(today);

            //when
            Ui.UserInput(string.Format("/wallet history --m"));
            var expectedOutput = new List<string>
            {
                "    when        where       howMuch  valueAfter",
                string.Empty, 
                "    2000-12-01  sourceName    +2.00        6.00"
            };

            //then
            Assert.That(ConsoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        public void ShouldDisplayFullHistory()
        {
            //given
            const string source = "sourceName";
            const string otherSource = "diffSource";
            Ui.UserInput(string.Format("/wallet source {0}", source));
            Ui.UserInput(string.Format("/wallet source {0}", otherSource));

            TimeMasterMock.SetupGet(mock => mock.Now).Returns(new DateTime(2000, 11, 30));
            Ui.UserInput(string.Format("/wallet add {0} 4", source));

            var today = new DateTime(2000, 12, 1);
            TimeMasterMock.SetupGet(mock => mock.Now).Returns(today);
            Ui.UserInput(string.Format("/wallet add {0} 2", source));
            Ui.UserInput(string.Format("/wallet sub {0} 1", source));
            Ui.UserInput(string.Format("/wallet trans {0} {1} 1", source, otherSource));

            TimeMasterMock.SetupGet(mock => mock.Today).Returns(today);

            //when
            Ui.UserInput(string.Format("/wallet history"));
            var expectedOutput = new List<string>
            {
                "    when        where                   howMuch  valueAfter",
                string.Empty,
                "    2000-11-30  sourceName                +4.00        4.00",
                "    2000-12-01  sourceName                +2.00        6.00",
                "    2000-12-01  sourceName                -1.00        5.00",
                "    2000-12-01  sourceName->diffSource     1.00            ",
                "                sourceName                -1.00        4.00",
                "                diffSource                +1.00        1.00"
            };

            //then
            Assert.That(ConsoleMock.Lines, Is.EquivalentTo(expectedOutput));
        }

        [Test]
        [TestCaseSource(typeof(TestCasesDataSource), "HistoryTests")]
        public void ShouldWork(IEnumerable<string> userCommands, IList<string> expectedOutput)
        {
            EndToEndTest(userCommands, expectedOutput);
        }

        private static class TestCasesDataSource
        {
            public static IEnumerable<TestCaseData> HistoryTests
            {
                get
                {
                    yield return new TestCaseData(new List<string>
                    {
                        "/wallet source mbank",
                        "/wallet add mbank 2 description tag2 tag3",
                        "/wallet source getin",
                        "/wallet trans mbank getin 1 'another description' tag3 taggg",
                        "/wallet history --t"
                    }, new List<string>
                    {
                        "    when        where         howMuch  valueAfter  tags       ",
                        string.Empty,
                        "    2015-05-24  mbank           +2.00        2.00  tag2, tag3 ",
                        "    2015-05-24  mbank->getin     1.00              tag3, taggg",
                        "                mbank           -1.00        1.00             ",
                        "                getin           +1.00        1.00             "
                    }).SetName("display history with tags");

                    yield return new TestCaseData(new List<string>
                    {
                        "/wallet source mbank",
                        "/wallet add mbank 2 description tag2 tag3",
                        "/wallet source getin",
                        "/wallet add getin 2",
                        "/wallet trans mbank getin 1 'another description' tag3 taggg",
                        "/wallet history getin"
                    }, new List<string>
                    {
                        "    when        where         howMuch  valueAfter",
                        string.Empty,
                        "    2015-05-24  getin           +2.00        2.00",
                        "    2015-05-24  mbank->getin     1.00            ",
                        "                mbank           -1.00        1.00",
                        "                getin           +1.00        3.00"
                    }).SetName("display history for specific source");
                }
            }
        }
    }
}
