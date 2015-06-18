namespace Modules.MoneyTracking.SchemaUpdates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Persistence;
    using Raven.Client;
    using Raven.Database.Storage;

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
                FillTagStringsAndStoreTags(session, actionOnOperationUpdated);
                RemoveDuplicatesFromTags(session, actionOnOperationUpdated);
            }
        }

        private void FillTagStringsAndStoreTags(IDocumentSession session, Action actionOnOperationUpdated)
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

            var operationsWithTagsNotInTagStrings = allOperations.Where(operation => operation.Tags != null);
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

        private void RemoveDuplicatesFromTags(IDocumentSession session, Action actionOnOperationUpdated)
        {
            var allTags = new List<Tag>();
            int start = 0;
            while (true)
            {
                var current = _ravenMagic.WaitForQueryIfNecessary(session.Query<Tag>()).Take(1024).Skip(start).ToList();
                if (current.Count == 0)
                {
                    break;
                }

                start += current.Count;
                allTags.AddRange(current);
            }

            var tagComparer = new Tag.Comparer();
            var distinctTags = allTags.Distinct(tagComparer);

            foreach (var distinctTag in distinctTags)
            {
                var tagsToRemove = allTags.Where(tag => tagComparer.Equals(tag, distinctTag)).Skip(1);

                foreach (var tagToRemove in tagsToRemove)
                {
                    session.Delete(tagToRemove);
                    actionOnOperationUpdated();
                }
            }
            session.SaveChanges();
        }
    }
}