﻿using Mailing.Module.Cache;
using StackExchange.Redis;

namespace Mailing.Module.Public;

public sealed class HasSenderApi(ConnectionMultiplexer multiplexer)
{
    public async Task<bool> HasSender()
    {
        MailingSendersCache cache = new(multiplexer);
        CachedMailingSender[] senders = await cache.GetAll();
        return senders.Length > 0;
    }
}
