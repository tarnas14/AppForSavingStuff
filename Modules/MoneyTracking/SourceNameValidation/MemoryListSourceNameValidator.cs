namespace Modules.MoneyTracking.SourceNameValidation
{
    using System.Collections.ObjectModel;

    public class MemoryListSourceNameValidator : SourceNameValidator
    {
        private readonly Collection<string> _reservedWords; 

        public MemoryListSourceNameValidator()
        {
            _reservedWords = new Collection<string>();
        }

        public void RestrictWord(string word)
        {
            _reservedWords.Add(word);
        }

        public void CheckIfValid(string sourceName)
        {
            if (Tag.IsTagName(sourceName))
            {
                throw new TagsNotAllowedAsSourceNameException();
            }

            if (_reservedWords.Contains(sourceName))
            {
                throw new SourceNameIsRestrictedException();
            }
        }
    }
}