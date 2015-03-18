namespace Specification.DataMigration
{
    using System.Collections.Generic;
    using System.Linq;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using NUnit.Framework;
    using Raven.Client;

    [TestFixture]
    public class CalculatingDifferenceForLegacyChangeDocuments
    {
        private DocumentStoreProvider _storeProvider;

        [SetUp]
        public void Setup()
        {
            _storeProvider = new DocumentStoreProvider() { RunInMemory = true };

            var legacyChange = new Change { Before = new Moneyz(0), After = new Moneyz(1), Source = "source" };
            var newChange = new Change { Difference = new Moneyz(2), Source = "source" };

            using (var session = _storeProvider.Store.OpenSession())
            {
                session.Store(new Operation { Changes = new[] { legacyChange } });
                session.Store(new Operation { Changes = new[] { newChange } });
                session.Store(new Operation());
                session.SaveChanges();
            }
        }

        [Test]
        public void ShouldQueryForOperationWithChangesWithoutDifference()
        {
            //given
            IList<Operation> operations;

            //when
            using (var session = _storeProvider.Store.OpenSession())
            {
                operations = DataMigrator.GetOperationsWithLegacyChanges(session);
            }

            //then
            Assert.That(operations.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ShouldUpdateLegacyChangesWithCalculatedDifference()
        {
            using (var session = _storeProvider.Store.OpenSession())
            {
                //given
                var operations = DataMigrator.GetOperationsWithLegacyChanges(session);

                //when
                DataMigrator.AddDifferencesAndRemoveBeforeProperties(operations);
                session.SaveChanges();
            }

            //then
            IEnumerable<Operation> legacyOperationsLeft;
            using (var session = _storeProvider.Store.OpenSession())
            {
                legacyOperationsLeft = DataMigrator.GetOperationsWithLegacyChanges(session);
            }

            Assert.That(legacyOperationsLeft.Count(), Is.EqualTo(0));
        }
    }

    public static class DataMigrator
    {
        public static IList<Operation> GetOperationsWithLegacyChanges(IDocumentSession session)
        {
            return session.Query<Operation>().Customize(q => q.WaitForNonStaleResultsAsOfNow())
                        .Where(operation => operation.Changes.Any(change => change.Before != null))
                        .ToList();
        }

        public static void AddDifferencesAndRemoveBeforeProperties(IList<Operation> operations)
        {
            var changes = operations.SelectMany(operation => operation.Changes);

            LegacyDataMagic.AddDifferencesToChanges(changes);
        }
    }
}