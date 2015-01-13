namespace Specification.UiSpec
{
    using System.Collections.Generic;
    using Halp;
    using NUnit.Framework;
    using Ui;

    [TestFixture]
    class ConsoleTableDisplaySpecification
    {
        private TableDisplay _tableDisplay;
        private ConsoleMock _consoleMock;

        [SetUp]
        public void Setup()
        {
            _consoleMock = new ConsoleMock();
            _tableDisplay = new TableDisplay(_consoleMock);
        }

        [Test]
        public void ShouldDisplayColumnHeaders()
        {
            //given
            _tableDisplay.AddColumn(new Column { Header = "First header" });
            _tableDisplay.AddColumn(new Column { Header = "Second header" });
            var expectedLines = new List<string>
            {
                "First headerSecond header"
            };

            //when
            _tableDisplay.Display();

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }

        [Test]
        public void ShouldAcceptMultipleColumns()
        {
            //given
            _tableDisplay.AddColumns(new List<Column>
            {
                new Column {Header = "First header"},
                new Column {Header = "Second header"}
            });
            var expectedLines = new List<string>
            {
                "First headerSecond header"
            };

            //when
            _tableDisplay.Display();

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }

        [Test]
        public void ShouldDisplayColumnsWithDataShorterThanHeadersWhenAllEqualHeight()
        {
            //given
            _tableDisplay.AddColumns(new List<Column>
            {
                new Column
                {
                    Header = "First header",
                    Data = new List<string>
                    {
                        "row1",
                        "row2"
                    }
                },
                new Column
                {
                    Header = "Second header",
                    Data = new List<string>
                    {
                        "row3",
                        "row4"
                    }
                }
            });
            var expectedLines = new List<string>
            {
                "First headerSecond header",
                "row1        row3         ",
                "row2        row4         "
            };

            //when
            _tableDisplay.Display();

            //then
            Assert.That(_consoleMock.Lines, Is.EquivalentTo(expectedLines));
        }
    }
}
