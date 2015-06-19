namespace Modules.MoneyTracking.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class SchemaUpdates
    {
        public static void MoveTagsToTagStrings(IEnumerable<Operation> operations, Action indicateProgress)
        {
            var operationsWithTagsNotInTagStrings = operations.Where(operation => operation.Tags != null);
            foreach (var operation in operationsWithTagsNotInTagStrings)
            {
                operation.TagStrings = new List<string>();

                foreach (var tag in operation.Tags)
                {
                    var sanitizedTagName = Tag.GetSanitizedValue(tag.Value);
                    operation.TagStrings.Add(sanitizedTagName);
                    indicateProgress();
                }
                operation.Tags = null;
            }
        }

        public static void PopulateOperationChangesWithBalanceDifferences(IEnumerable<Operation> operations,
            Action indicateProgress)
        {
            var operationsWithoutDifference = operations.Where(operation => operation.Changes.Any(change => change.Difference == null));
            foreach (var change in operationsWithoutDifference.SelectMany(operation => operation.Changes))
            {
                change.Difference = change.After - change.Before;
                indicateProgress();
            }
        }

        public static void InitializeOperationsWithEmptyTagStringsCollections(IEnumerable<Operation> operations,
            Action indicateProgress)
        {
            var operationsWithUninitializedTagStrings = operations.Where(operation => operation.TagStrings == null);

            foreach (var operationWithUninitializedTagStrings in operationsWithUninitializedTagStrings)
            {
                operationWithUninitializedTagStrings.TagStrings = new List<string>();
            }
        }

        public static IEnumerable<Tag> FindDuplicatedTagsToRemove(IEnumerable<Tag> tags, Action indicatePRogress)
        {

            var tagComparer = new Tag.Comparer();
            var distinctTags = tags.Distinct(tagComparer);

            var tagsToRemove = new List<Tag>();
            foreach (var distinctTag in distinctTags)
            {
                tagsToRemove.AddRange(tags.Where(tag => tagComparer.Equals(tag, distinctTag)).Skip(1));
            }

            return tagsToRemove;
        }

        public static IEnumerable<Tag> SanitizeTags(IEnumerable<Tag> dirtyTags)
        {
            return dirtyTags.Select(dirtyTag =>
            {
                if (!Tag.IsTagName(dirtyTag.Value))
                {
                    return new Tag(dirtyTag.Value);
                }

                return dirtyTag;
            });
        }
    }
}