namespace Modules.MoneyTracking.CommandHandlers
{
    using System.Collections.Generic;

    public class HardcodedReservedWordsStore : ReservedWordsStore
    {
        private static readonly List<string> ReservedWords = new List<string>
            {
                "tags",
                "all"
            };

        public bool IsReserved(string word)
        {
            return ReservedWords.Contains(word);
        }
    }
}