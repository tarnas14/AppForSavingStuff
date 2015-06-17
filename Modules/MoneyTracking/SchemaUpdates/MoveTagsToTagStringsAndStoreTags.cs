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
                IEnumerable<Operation> operations;
                while ((operations = GetOperationsWithTagsNotInTagStrings(session)).Any())
                {
                    foreach (var operation in operations.Where(operation => operation.Tags != null))
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
        }

        private IEnumerable<Operation> GetOperationsWithTagsNotInTagStrings(IDocumentSession session)
        {
            return _ravenMagic.WaitForQueryIfNecessary(session.Query<Operation>()).Where(operation => operation.Tags != null);
        }
    }
}