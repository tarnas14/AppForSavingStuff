namespace Modules.MoneyTracking.Persistence
{
    using Raven.Client;
    using Raven.Client.Linq;

    public interface BagOfRavenMagic
    {
        IDocumentStore Store { get; }
        IRavenQueryable<TEntity> WaitForQueryIfNecessary<TEntity>(IRavenQueryable<TEntity> query) where TEntity : class;
    }
}