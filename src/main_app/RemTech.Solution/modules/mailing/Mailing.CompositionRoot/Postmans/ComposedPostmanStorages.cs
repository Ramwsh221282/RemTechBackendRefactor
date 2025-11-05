using Mailing.Domain.Postmans;
using Mailing.Domain.Postmans.Storing;

namespace Mailing.CompositionRoot.Postmans;

public sealed class ComposedPostmanStorages : IPostmansStorage
{
    private readonly List<IPostmansStorage> _storages = [];

    public ComposedPostmanStorages Add(IPostmansStorage storage)
    {
        _storages.Add(storage);
        return this;
    }

    public void Save(int sendLimit, int currentSend)
    {
        foreach (var storage in _storages)
            storage.Save(sendLimit, currentSend);
    }

    public void Save(Guid id, string email, string password)
    {
        foreach (var storage in _storages)
            storage.Save(id, email, password);
    }

    public void Save(IPostman postman)
    {
        foreach (var storage in _storages)
            postman.Save(storage);
    }

    public void Save(Action<IPostmanMetadataStorage, IPostmanStatisticsStorage> saveDelegate)
    {
        foreach (var storage in _storages)
            storage.Save(saveDelegate);
    }
}