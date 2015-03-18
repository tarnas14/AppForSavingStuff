namespace Specification.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using NUnit.Framework;

    class CalculatingSourcesFromChangesViaMapReduceSpecification
    {
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
            IList<Sources_ByChangesInOperations.Result> sources;
            using (var session = provider.Store.OpenSession())
            {
                sources = session.Query<Sources_ByChangesInOperations.Result, Sources_ByChangesInOperations>().Customize(q => q.WaitForNonStaleResults()).Where(s => s.Name == "asdf").ToList();
            }

            //then
            var source = sources.SingleOrDefault();
            Assert.That(source, Is.Not.Null);
            Assert.That(source.Balance, Is.EqualTo(4));
        }

        [Test]
        public void ShouldMapReduceSourcesFromChangesStoredInOperationsTakingLegacyDataIntoAccount()
        {
            //given
            var provider = new DocumentStoreProvider() { RunInMemory = true };

            new Sources_ByChangesInOperations().Execute(provider.Store);

            var opreation1 = new Operation(DateTime.Now).AddChange("asdf", new Moneyz(2));
            var operation2 = new Operation(DateTime.Now).AddChange("asdf", new Moneyz(2));
            var legacyOperation = new Operation(DateTime.Now)
            {
                Changes = new[] { new Change { Source = "asdf", Before = new Moneyz(0), After = new Moneyz(2) } }
            };

            using (var session = provider.Store.OpenSession())
            {
                session.Store(opreation1);
                session.Store(operation2);
                session.Store(legacyOperation);

                session.SaveChanges();
            }

            //when
            IList<Sources_ByChangesInOperations.Result> sources;
            using (var session = provider.Store.OpenSession())
            {
                sources = session.Query<Sources_ByChangesInOperations.Result, Sources_ByChangesInOperations>().Customize(q => q.WaitForNonStaleResults()).Where(s => s.Name == "asdf").ToList();
            }

            //then
            var source = sources.SingleOrDefault();
            Assert.That(source, Is.Not.Null);
            Assert.That(source.Balance, Is.EqualTo(6));
        }
    }
}
