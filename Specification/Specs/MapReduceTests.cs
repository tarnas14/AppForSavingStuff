namespace Specification.Specs
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using NUnit.Framework;

    class MapReduceTests
    {
        [Test]
        public void ShouldMapReduceSourcesFromStoredChanges()
        {
            //given
            var provider = new DocumentStoreProvider() {RunInMemory = true};

            var change1 = new Change() {Source = "asdf", Difference = new Moneyz(2)};
            var change2 = new Change() { Source = "asdf", Difference = new Moneyz(2) };

            using (var session = provider.Store.OpenSession())
            {
                session.Store(change1);
                session.Store(change2);
                session.SaveChanges();
            }

            //when
            IList<Source> sources;
            using (var session = provider.Store.OpenSession())
            {
                sources = session.Query<Source, Sources_ByChanges>().Customize(q => q.WaitForNonStaleResultsAsOfNow()).ToList();
            }

            //then
            var source = sources.SingleOrDefault();
            Assert.That(source, Is.Not.Null);
            Assert.That(source.Balance, Is.EqualTo(new Moneyz(4)));
        }
    }
}
