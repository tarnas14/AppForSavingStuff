namespace Modules.MoneyTracking.Persistence
{
    using System.Linq;
    using Raven.Client.Indexes;

    public class Sources_ByName : AbstractIndexCreationTask<Source>
    {
        public Sources_ByName()
        {
            Map = sources => from source in sources
                select new 
                {
                    Name = source.Name
                };
        }
    }
}