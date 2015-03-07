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
            Ui.UserInput(string.Format("/wallet add {0} 4 -date 2000-11-30", source));
            Ui.UserInput(string.Format("/wallet add {0} 2 -date 2000-12-01", source));

            var today = new DateTime(2000, 12, 1);
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
            Ui.UserInput(string.Format("/wallet add {0} 4 -date 2000-11-30", source));
            Ui.UserInput(string.Format("/wallet add {0} 2 -date 2000-12-01", source));
            Ui.UserInput(string.Format("/wallet sub {0} 1 -date 2000-12-01", source));
            Ui.UserInput(string.Format("/wallet trans {0} {1} 1 -date 2000-12-01", source, otherSource));

            var today = new DateTime(2000, 12, 1);
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
                        "    when        where         howMuch  valueAfter  tags        ",
                        string.Empty,
                        "    2015-05-24  mbank           +2.00        2.00  #tag2 #tag3 ",
                        "    2015-05-24  mbank->getin     1.00              #tag3 #taggg",
                        "                mbank           -1.00        1.00              ",
                        "                getin           +1.00        1.00              "
                    }).SetName("display history with tags");

                    yield return new TestCaseData(new []
                    {
                        "/wallet source mbank",
                        "/wallet source getin",
                        "/wallet add mbank 2 short",
                        "/wallet sub mbank 1 'other description'",
                        "/wallet trans mbank getin 1 'trans description'",
                        "/wallet history --d"
                    }, new []
                    {
                        "    when        where         howMuch  valueAfter  description        ",
                        string.Empty,
                        "    2015-05-24  mbank           +2.00        2.00  'short'            ",
                        "    2015-05-24  mbank           -1.00        1.00  'other description'",
                        "    2015-05-24  mbank->getin     1.00              'trans description'",
                        "                mbank           -1.00        0.00                     ",
                        "                getin           +1.00        1.00                     "
                    }).SetName("display history with descriptions");

                    yield return new TestCaseData(new[]
                    {
                        "/wallet source mbank",
                        "/wallet source getin",
                        "/wallet add mbank 2 'some very long description longer than 30'",
                        "/wallet sub mbank 1 'short description lol'",
                        "/wallet trans mbank getin 1 'another quite long description to make the point of shortening it for display'",
                        "/wallet history --d"
                    }, new[]
                    {
                        "    when        where         howMuch  valueAfter  description                     ",
                        string.Empty,
                        "    2015-05-24  mbank           +2.00        2.00  'some very long description l..'",
                        "    2015-05-24  mbank           -1.00        1.00  'short description lol'         ",
                        "    2015-05-24  mbank->getin     1.00              'another quite long descripti..'",
                        "                mbank           -1.00        0.00                                  ",
                        "                getin           +1.00        1.00                                  "
                    }).SetName("descriptions longer than 30 characters should be shortened to 28with 2 dots suffix");

                    yield return new TestCaseData(new[]
                    {
                        "/wallet source mbank",
                        "/wallet source getin",
                        "/wallet add mbank 2 short tag1 tag2",
                        "/wallet sub mbank 1 'other description' tag3 tag4",
                        "/wallet trans mbank getin 1 'trans description' tag5 tag6",
                        "/wallet history --d --t"
                    }, new[]
                    {
                        "    when        where         howMuch  valueAfter  description          tags       ",
                        string.Empty,
                        "    2015-05-24  mbank           +2.00        2.00  'short'              #tag1 #tag2",
                        "    2015-05-24  mbank           -1.00        1.00  'other description'  #tag3 #tag4",
                        "    2015-05-24  mbank->getin     1.00              'trans description'  #tag5 #tag6",
                        "                mbank           -1.00        0.00                                  ",
                        "                getin           +1.00        1.00                                  "
                    }).SetName("display history with descriptions and tags");

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

                    yield return new TestCaseData(new []
                    {
                        "/wallet source src1",
                        "/wallet source src2",
                        "/wallet source src3",
                        "/wallet add src1 1",
                        "/wallet add src2 2",
                        "/wallet add src3 3",
                        "/wallet history src1 src3"
                    }, new []
                    {
                        "    when        where  howMuch  valueAfter",
                        string.Empty,
                        "    2015-05-24  src1     +1.00        1.00",
                        "    2015-05-24  src3     +3.00        3.00",
                    }).SetName("display history for multiple sources");
                }
            }
        }
    }
}
