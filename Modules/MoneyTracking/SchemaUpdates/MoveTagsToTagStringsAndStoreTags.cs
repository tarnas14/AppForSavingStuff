namespace Modules.MoneyTracking.SchemaUpdates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Raven.Client;

    public class MoveTagsToTagStringsAndStoreTags
    {
        public void Update(IDocumentSession session, Action actionOnOperationUpdated)
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

        private IEnumerable<Operation> GetOperationsWithTagsNotInTagStrings(IDocumentSession session)
        {
            return session.Query<Operation>().Where(operation => operation.Tags != null);
        }
    }
}