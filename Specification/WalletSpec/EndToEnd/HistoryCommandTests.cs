namespace Specification.WalletSpec.EndToEnd
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    class HistoryCommandTests
    {
        private EndToEndTester _endToEnd;

        [SetUp]
        public void Setup()
        {
            _endToEnd = new EndToEndTester();
            _endToEnd.SetTime(new DateTime(2015, 5, 24));
        }

        [Test]
        public void ShouldDisplayHistoryForCurrentMonth()
        {
            //given
            const string source = "sourceName";
            _endToEnd.Execute(string.Format("/wallet add {0} 4 -date 2000-11-30", source));
            _endToEnd.Execute(string.Format("/wallet add {0} 2 -date 2000-12-01", source));

            _endToEnd.SetTime(new DateTime(2000, 12, 1));

            //when
            _endToEnd.Execute("/wallet history --m");

            //then
            _endToEnd.AssertExpectedResult(
                "    when        where       howMuch  valueAfter",
                string.Empty,
                "    2000-12-01  sourceName    +2.00        6.00");
        }

        [Test]
        [TestCase("/wallet history --m -month 2001-01")]
        [TestCase("/wallet history -month 2001-01")]
        public void ShouldDisplayHistoryForSpecifiedMonth(string command)
        {
            //given
            _endToEnd.Execute("/wallet add source 4 -date 2001-01-01");
            _endToEnd.Execute("/wallet add source 4 -date 2001-02-01");

            _endToEnd.SetTime(new DateTime(2001, 2, 2));

            //when
            _endToEnd.Execute(command);

            //then
            _endToEnd.AssertExpectedResult(
                "    when        where   howMuch  valueAfter",
                string.Empty,
                "    2001-01-01  source    +4.00        4.00");
        }

        [Test]
        public void ShouldDisplayFullHistory()
        {
            //given
            const string source = "sourceName";
            const string otherSource = "diffSource";
            _endToEnd.Execute(string.Format("/wallet add {0} 4 -date 2000-11-30", source));
            _endToEnd.Execute(string.Format("/wallet add {0} 2 -date 2000-12-01", source));
            _endToEnd.Execute(string.Format("/wallet sub {0} 1 -date 2000-12-01", source));
            _endToEnd.Execute(string.Format("/wallet trans {0} {1} 1 -date 2000-12-01", source, otherSource));

            _endToEnd.SetTime(new DateTime(2000, 12, 1));

            //when
            _endToEnd.Execute("/wallet history");

            //then
            _endToEnd.AssertExpectedResult(
                "    when        where                   howMuch  valueAfter",
                string.Empty,
                "    2000-11-30  sourceName                +4.00        4.00",
                "    2000-12-01  sourceName                +2.00        6.00",
                "    2000-12-01  sourceName                -1.00        5.00",
                "    2000-12-01  sourceName->diffSource     1.00            ",
                "                sourceName                -1.00        4.00",
                "                diffSource                +1.00        1.00"
            );
        }

        [Test]
        public void ShouldDisplayHistoryWithTags()
        {
            //given
            _endToEnd.Execute("/wallet add mbank 2 description tag2 tag3");
            _endToEnd.Execute("/wallet trans mbank getin 1 'another description' tag3 taggg");

            //when
            _endToEnd.Execute("/wallet history --t");

            //then
            _endToEnd.AssertExpectedResult(
                "    when        where         howMuch  valueAfter  tags        ",
                string.Empty,
                "    2015-05-24  mbank           +2.00        2.00  #tag2 #tag3 ",
                "    2015-05-24  mbank->getin     1.00              #tag3 #taggg",
                "                mbank           -1.00        1.00              ",
                "                getin           +1.00        1.00              "
                );
        }

        [Test]
        public void ShouldDisplayHistoryWithDescriptions()
        {
            //given
            _endToEnd.Execute("/wallet add mbank 2 short");
            _endToEnd.Execute("/wallet sub mbank 1 'other description'");
            _endToEnd.Execute("/wallet trans mbank getin 1 'trans description'");

            //when
            _endToEnd.Execute("/wallet history --d");

            //then
            _endToEnd.AssertExpectedResult(
                "    when        where         howMuch  valueAfter  description        ",
                string.Empty,
                "    2015-05-24  mbank           +2.00        2.00  'short'            ",
                "    2015-05-24  mbank           -1.00        1.00  'other description'",
                "    2015-05-24  mbank->getin     1.00              'trans description'",
                "                mbank           -1.00        0.00                     ",
                "                getin           +1.00        1.00                     ");
        }

        [Test]
        public void ShouldShortenDescriptionsTo30Characters()
        {
            //given
            _endToEnd.Execute("/wallet add mbank 2 'some very long description longer than 30'");
            _endToEnd.Execute("/wallet sub mbank 1 'short description lol'");
            _endToEnd.Execute("/wallet trans mbank getin 1 'another quite long description to make the point of shortening it for display'");

            //when
            _endToEnd.Execute("/wallet history --d");

            //then
            _endToEnd.AssertExpectedResult(
                "    when        where         howMuch  valueAfter  description                     ",
                string.Empty,
                "    2015-05-24  mbank           +2.00        2.00  'some very long description l..'",
                "    2015-05-24  mbank           -1.00        1.00  'short description lol'         ",
                "    2015-05-24  mbank->getin     1.00              'another quite long descripti..'",
                "                mbank           -1.00        0.00                                  ",
                "                getin           +1.00        1.00                                  "
                );
        }

        [Test]
        public void ShouldDisplayHistoryWithBothDescriptionsAndTags()
        {
            //given
            _endToEnd.Execute("/wallet add mbank 2 short tag1 tag2");
            _endToEnd.Execute("/wallet sub mbank 1 'other description' tag3 tag4");
            _endToEnd.Execute("/wallet trans mbank getin 1 'trans description' tag5 tag6");

            //when
            _endToEnd.Execute("/wallet history --d --t");

            //then
            _endToEnd.AssertExpectedResult(
                "    when        where         howMuch  valueAfter  description          tags       ",
                string.Empty,
                "    2015-05-24  mbank           +2.00        2.00  'short'              #tag1 #tag2",
                "    2015-05-24  mbank           -1.00        1.00  'other description'  #tag3 #tag4",
                "    2015-05-24  mbank->getin     1.00              'trans description'  #tag5 #tag6",
                "                mbank           -1.00        0.00                                  ",
                "                getin           +1.00        1.00                                  "
                );
        }

        [Test]
        public void ShouldDisplayHistoryForSpecificSource()
        {
            //given
            _endToEnd.Execute("/wallet add mbank 2 description tag2 tag3");
            _endToEnd.Execute("/wallet add getin 2");
            _endToEnd.Execute("/wallet trans mbank getin 1 'another description' tag3 taggg");

            //when
            _endToEnd.Execute("/wallet history getin");

            //then
            _endToEnd.AssertExpectedResult(
                "    when        where         howMuch  valueAfter",
                string.Empty,
                "    2015-05-24  getin           +2.00        2.00",
                "    2015-05-24  mbank->getin     1.00            ",
                "                mbank           -1.00        1.00",
                "                getin           +1.00        3.00"
                );
        }

        [Test]
        public void ShouldDisplayHistoryForMultipleSources()
        {
            //given
            _endToEnd.Execute("/wallet add src1 1");
            _endToEnd.Execute("/wallet add src2 2");
            _endToEnd.Execute("/wallet add src3 3");

            //when
            _endToEnd.Execute("/wallet history src1 src3");

            //then
            _endToEnd.AssertExpectedResult(
                "    when        where  howMuch  valueAfter",
                string.Empty,
                "    2015-05-24  src1     +1.00        1.00",
                "    2015-05-24  src3     +3.00        3.00"
                );
        }
    }
}
