namespace Modules.MoneyTracking.SchemaUpdates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Persistence;
    using Raven.Client;

    public class MoveTagsToTagStringsAndStoreTags
    {
        private readonly BagOfRavenMagic _ravenMagic;

        public MoveTagsToTagStringsAndStoreTags(BagOfRavenMagic ravenMagic)
        {
            _ravenMagic = ravenMagic;
        }

        public void Update(Action actionOnOperationUpdated)
        {
            using (var session = _ravenMagic.Store.OpenSession())
            {
                var operationsWithTagsNotInTagStrings = GetOperationsWithTagsNotInTagStrings(session);
                foreach (var operation in operationsWithTagsNotInTagStrings)
                {
                    operation.TagStrings = new List<string>();
                    operation.Tags.ToList().ForEach(tag =>
                    {
                        var sanitizedTag = new Tag(Tag.IsTagName(tag.Value) ? tag.Value : "#" + tag.Value);
                        operation.TagStrings.Add(sanitizedTag.Value);
                        session.Store(sanitizedTag);
                        actionOnOperationUpdated();
                    });
                    operation.Tags = null;
                }
                session.SaveChanges();
            }
        }

        private IEnumerable<Operation> GetOperationsWithTagsNotInTagStrings(IDocumentSession session)
        {
            var allOperations = new List<Operation>();
            int start = 0;
            while (true)
            {
                var current = _ravenMagic.WaitForQueryIfNecessary(session.Query<Operation>()).Take(1024).Skip(start).ToList();
                if (current.Count == 0)
                {
                    break;
                }

                start += current.Count;
                allOperations.AddRange(current);
            }

            return _ravenMagic.WaitForQueryIfNecessary(session.Query<Operation>()).Where(operation => operation.Tags != null);
        }
    }
}