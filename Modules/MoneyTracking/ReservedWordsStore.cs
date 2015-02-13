namespace Modules.MoneyTracking
{
    public interface ReservedWordsStore
    {
        bool IsReserved(string word);
    }
}