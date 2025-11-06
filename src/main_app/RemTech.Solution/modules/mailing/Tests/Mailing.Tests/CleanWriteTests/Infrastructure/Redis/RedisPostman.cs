using Shared.Infrastructure.Module.Redis;
using StackExchange.Redis;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.Redis;

public sealed class RedisPostman(RedisCache cache)
    : IWritePostmanMetadataInfrastructureCommand, IWritePostmanStatisticsInfrastructureCommand
{
    private readonly IDatabase _database = cache.Database;
    private JsonPostman _postman = new();

    public async Task Save() =>
        await _postman.Save(_database);

    public void Execute(in Guid id, in string email, in string password)
    {
        _postman.AddId(id);
        _postman.AddEmail(email);
        _postman.AddPassword(password);
    }

    public void Execute(in int sendLimit, in int currentSend)
    {
        _postman.AddLimitSend(sendLimit);
        _postman.AddCurrentSend(currentSend);
    }
}