namespace Specification.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using NUnit.Framework;
    using Raven.Client.Indexes;

    class MapReduceExploration
    {
        [Test]
        public void ShouldMapReduceSourcesFromStoredChanges()
        {
            //given
            var provider = new DocumentStoreProvider() {RunInMemory = true};

            new Sources_ByChanges().Execute(provider.Store);

            var change1 = new Change() { Source = "asdf", Difference = new Moneyz(2) };
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
                sources = session.Query<Source, Sources_ByChanges>().Customize(q => q.WaitForNonStaleResultsAsOfNow()).Where(s => s.Name == "asdf").ToList();
            }

            //then
            var source = sources.SingleOrDefault();
            Assert.That(source, Is.Not.Null);
            Assert.That(source.Balance, Is.EqualTo(new Moneyz(4)));
        }

        [Test]
        public void ShouldMapReduceSourcesFromChangesStoredInOperations()
        {
            //given
            var provider = new DocumentStoreProvider() { RunInMemory = true };

            new Sources_ByChangesInOperations().Execute(provider.Store);

            var opreation1 = new Operation(DateTime.Now).AddChange("asdf", new Moneyz(2));
            var operation2 = new Operation(DateTime.Now).AddChange("asdf", new Moneyz(2));

            using (var session = provider.Store.OpenSession())
            {
                session.Store(opreation1);
                session.Store(operation2);
                session.SaveChanges();
            }

            //when
            IList<Source> sources;
            using (var session = provider.Store.OpenSession())
            {
                sources = session.Query<Source, Sources_ByChangesInOperations>().Customize(q => q.WaitForNonStaleResultsAsOfNow()).Where(s => s.Name == "asdf").ToList();
            }

            //then
            var source = sources.SingleOrDefault();
            Assert.That(source, Is.Not.Null);
            Assert.That(source.Balance, Is.EqualTo(new Moneyz(4)));
        }
    }

    internal class Sources_ByChangesInOperations : AbstractIndexCreationTask<Operation, Source>
    {
        public override bool IsMapReduce
        {
            get { return true; }
        }

        public Sources_ByChangesInOperations()
        {
            Map = operations => from operation in operations
                from change in operation.Changes
                select new Source
                {
                    Name = change.Source,
                    Balance = change.Difference
                };
            Reduce = results => from result in results
                group result by result.Name
                into g
                select new Source
                {
                    Name = g.Key,
                    Balance = g.Sum(x => x.Balance)
                };
        }
    }

    internal class Sources_ByChanges : AbstractIndexCreationTask<Change, Source>
    {
        public override bool IsMapReduce
        {
            get { return true; }
        }

        public Sources_ByChanges()
        {
            Map = changes => from change in changes
                             select new Source
                             {
                                 Name = change.Source,
                                 Balance = change.Difference
                             };

            Reduce = results => from result in results
                                group result by result.Name
                                    into g
                                    select new Source
                                    {
                                        Name = g.Key,
                                        Balance = g.Sum(x => x.Balance)
                                    };
        }
    }
}
