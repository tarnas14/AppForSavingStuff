namespace Specification
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using NUnit.Framework;

    [TestFixture]
    class StuffRepositorySpec
    {
        private StuffRepository _repo;

        [SetUp]
        public void Setup()
        {
            _repo = new StuffRepository();
        }

        [Test]
        public void ShouldSaveStuff()
        {
            //given
            var stuff = new Stuff
            {
                When = DateTime.Now
            };

            //when
            _repo.Save(stuff);
            var forDay = _repo.GetForDay(DateTime.Today);

            //then
            Assert.That(forDay.Last(), Is.EqualTo(stuff));
        }

        [Test]
        public void ShouldReturnStuffForDayOrderedByDate()
        {
            //given
            var stuff = new Stuff
            {
                When = new DateTime(2012, 10, 12, 12, 0, 0)
            };
            var earlierStuff = new Stuff
            {
                When = new DateTime(2012, 10, 12, 4, 0, 0)
            };
            var muchEarlierStuff = new Stuff
            {
                When = DateTime.Now.Subtract(TimeSpan.FromDays(1))
            };

            //when
            _repo.Save(stuff);
            _repo.Save(muchEarlierStuff);
            _repo.Save(earlierStuff);
            var forDay = _repo.GetForDay(stuff.When);

            //then
            Assert.That(forDay.Count(), Is.EqualTo(2));
            Assert.That(forDay.Last(), Is.EqualTo(stuff));
        }
    }

    internal class StuffRepository
    {
        private readonly ICollection<Stuff> _stuff;

        public StuffRepository()
        {
            _stuff = new Collection<Stuff>();
        }

        public void Save(Stuff stuff)
        {
            _stuff.Add(stuff);
        }

        public IEnumerable<Stuff> GetForDay(DateTime day)
        {
            return OrderByDate(_stuff.Where(s => s.When.Date.Equals(day.Date)));
        }

        private IEnumerable<Stuff> OrderByDate(IEnumerable<Stuff> stuff)
        {
            return stuff.OrderBy(s => s.When);
        }
    }

    internal class Stuff
    {
        public DateTime When { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", When.ToShortDateString());
        }
    }
}
