namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;

    public class HardcodedReservedWordsStore : ReservedWordsStore
    {
        private static readonly List<string> ReservedWords = new List<string>
            {
                "tags"
            };

        public bool IsReserved(string word)
        {
            return ReservedWords.Contains(word.ToLower());
        }
    }
}