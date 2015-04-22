namespace Specification.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Modules.MoneyTracking;
    using Modules.MoneyTracking.Persistence;
    using NUnit.Framework;

    class RetrievingOperationsBySourceNameSpecification
    {
        [Test]
        public void ShouldGetSingleOperationThatHaveChangesOnSpecifiedSource()
        {
            //given
            var provider = new DocumentStoreProvider() { RunInMemory = true };

            new Operations_BySources().Execute(provider.Store);

            const string expectedSourceName = "asdf";
            var operation1 = new Operation(DateTime.Now).AddChange(expectedSourceName, new Moneyz(2));
            var operation2 = new Operation(DateTime.Now).AddChange("qwer", new Moneyz(2));

            using (var session = provider.Store.OpenSession())
            {
                session.Store(operation1);
                session.Store(operation2);

                session.SaveChanges();
            }

            //when
            IList<Operation> operations;
            using (var session = provider.Store.OpenSession())
            {
                operations = session.Query<Operations_BySources.Result, Operations_BySources>().
                    Customize(q => q.WaitForNonStaleResults()).Where(op => op.SourceName == expectedSourceName).
                    OfType<Operation>().ToList();
            }

            //then
            var operation = operations.Single();
            Assert.That(operation.Changes.Single().Source, Is.EqualTo(expectedSourceName));
        }
    }
}
