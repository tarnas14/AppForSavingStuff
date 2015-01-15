namespace Specification.UiSpec
{
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;
    using Ui;

    [TestFixture]
    class ColumnSpecification
    {
        [Test]
        public void ShouldGiveColumnWithOnlyHeaderWidth()
        {
            //given
            var column = new Column
            {
                Header = "123456789"
            };

            //then
            Assert.That(column.Width, Is.EqualTo(9));
        }

        [Test]
        public void ShouldGiveWidthWithDataLongerThanHeader()
        {
            //given
            var column = new Column
            {
                Header = "123456789",
                Data = new List<string>
                {
                    "1234567890",
                    "1234"
                }
            };

            //then
            Assert.That(column.Width, Is.EqualTo(10));
        }

        [Test]
        public void ShouldCalculateWidthWithoutHeader()
        {
            //given
            var column = new Column
            {
                Data = new List<string>
                {
                    "1234567890",
                    "1234"
                }
            };

            //then
            Assert.That(column.Width, Is.EqualTo(10));
        }

        [Test]
        public void ShouldGiveColumnHeightAsCountOfDataRows()
        {
            //given
            var column = new Column
            {
                Header = "123456789",
                Data = new List<string>
                {
                    "1234567890",
                    "asdf"
                }
            };

            //when
            var height = column.Height;

            //then
            Assert.That(height, Is.EqualTo(2));
        }

        [Test]
        public void ShouldGiveHeader()
        {
            //given
            var columnLeftAlign = new Column
            {
                Header = "123456789",
                Prefix = "-",
                Suffix = "-",
                Data = new[] { "1234567890" }
            };

            //when
            var header = columnLeftAlign.GetHeader();

            //then
            Assert.That(header, Is.EqualTo("-123456789 -"));
        }

        [Test]
        public void ShouldNotGiveHeaderAsFirstRow()
        {
            //given
            var column = new Column
            {
                Header = "123456789",
                Data = new List<string>
                {
                    "1234567890"
                }
            };

            //when
            var firstRow = column.GetDataRow(0);

            //then
            Assert.That(firstRow, Is.EqualTo("1234567890"));
        }

        [Test]
        public void ShouldAccessDataRows()
        {
            //given
            var column = new Column
            {
                Header = "1234567890",
                Data = new List<string>
                {
                    "123456789"
                }
            };

            //when
            var firstRow = column.GetDataRow(0);

            //then
            Assert.That(firstRow, Is.EqualTo("123456789 "));
        }

        [Test]
        public void ShouldReturnStringOfWidthLengthIfIdOutOfBounds()
        {
            //given
            var column = new Column
            {
                Header = "1234567890",
                Data = new List<string>
                {
                    "123456789"
                }
            };

            //when
            var negativeRow = column.GetDataRow(-1);
            var tooHighIdRow = column.GetDataRow(2);

            //then
            Assert.That(negativeRow.Length, Is.EqualTo(10));
            Assert.That(string.IsNullOrWhiteSpace(negativeRow));
            Assert.That(tooHighIdRow.Length, Is.EqualTo(10));
            Assert.That(string.IsNullOrWhiteSpace(tooHighIdRow));
        }

        [Test]
        public void ShouldAddPrefixToEachColumn()
        {
            //given
            var column = new Column
            {
                Prefix = "--",
                Header = "1234567890",
                Data = new List<string>
                {
                    "123456789"
                }
            };

            //when

            //then
            Assert.That(column.GetDataRows().All(row => row.StartsWith(column.Prefix)));
        }

        [Test]
        public void ShouldAddSuffixToEachColumn()
        {
            //given
            var column = new Column
            {
                Suffix = "--",
                Header = "1234567890",
                Data = new List<string>
                {
                    "123456789"
                }
            };

            //when

            //then
            Assert.That(column.GetDataRows().All(row => row.EndsWith(column.Suffix)));
        }

        [Test]
        public void ShouldAlignColumnsToTheRight()
        {
            //given
            var column = new Column
            {
                Header = "1234567890",
                Data = new List<string>
                {
                    "123456789"
                },
                AlignRight = true
            };

            //when

            //then
            Assert.That(column.GetDataRow(0), Is.EqualTo(" 123456789"));
        }

        [Test]
        public void ShouldAddSuffixAfterRightAlignment()
        {
            //given
            var column = new Column
            {
                Header = "1234567890",
                Data = new List<string>
                {
                    "1234"
                },
                AlignRight = true,
                Suffix = "--"
            };

            //when

            //then
            Assert.That(column.GetHeader(), Is.EqualTo("1234567890--"));
            Assert.That(column.GetDataRow(0), Is.EqualTo("      1234--"));
        }
    }
}
