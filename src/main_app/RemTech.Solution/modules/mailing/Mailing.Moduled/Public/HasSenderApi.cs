using Mailing.Moduled.Cache;
using Shared.Infrastructure.Module.Redis;

namespace Mailing.Moduled.Public;

public sealed class HasSenderApi(RedisCache cache)
{
    public async Task<bool> HasSender()
    {
        MailingSendersCache cache = new(multiplexer);
        CachedMailingSender[] senders = await cache.GetAll();
        return senders.Length > 0;
    }

    public async Task HasSenderAndThrowIfNotExists()
    {
        bool hasSender = await HasSender();
        if (!hasSender)
            throw new HasNoAvailableSendersException();
    }
}