﻿using System.Text.Json;
using Mailing.Module.Contracts;
using StackExchange.Redis;

namespace Mailing.Module.Cache;

internal sealed class MailingSendersCache(ConnectionMultiplexer multiplexer)
{
    private const string Key = "Senders";

    public async Task<CachedMailingSender[]> GetAll()
    {
        IDatabase db = multiplexer.GetDatabase();
        string? array = await db.StringGetAsync(Key);
        return string.IsNullOrWhiteSpace(array)
            ? []
            : JsonSerializer.Deserialize<CachedMailingSender[]>(array)!;
    }

    public async Task Add(IEmailSender sender)
    {
        IDatabase db = multiplexer.GetDatabase();
        string? array = await db.StringGetAsync(Key);
        if (string.IsNullOrWhiteSpace(array))
        {
            await CreateNewArray(sender, db);
            return;
        }
        await AddInArray(array, sender, db);
    }

    public async Task Update(IEmailSender sender)
    {
        IDatabase db = multiplexer.GetDatabase();
        string? array = await db.StringGetAsync(Key);
        if (string.IsNullOrWhiteSpace(array))
            return;
        await UpdateInArray(array, sender, db);
    }

    public async Task Remove(IEmailSender sender)
    {
        IDatabase db = multiplexer.GetDatabase();
        string? array = await db.StringGetAsync(Key);
        if (string.IsNullOrWhiteSpace(array))
            return;
        await RemoveFromArray(array, sender, db);
    }

    private static async Task CreateNewArray(IEmailSender sender, IDatabase database)
    {
        CachedMailingSender cached = CachedMailingSender.FromSender(sender);
        CachedMailingSender[] senders = [cached];
        await UpdateArray(senders, database);
    }

    private static async Task RemoveFromArray(string json, IEmailSender sender, IDatabase database)
    {
        CachedMailingSender[] senders = JsonSerializer.Deserialize<CachedMailingSender[]>(json)!;
        CachedMailingSender toRemove = CachedMailingSender.FromSender(sender);
        senders = senders.Where(s => s.Email != toRemove.Email).ToArray();
        await UpdateArray(senders, database);
    }

    private static async Task UpdateInArray(string json, IEmailSender sender, IDatabase database)
    {
        CachedMailingSender[] senders = JsonSerializer.Deserialize<CachedMailingSender[]>(json)!;
        CachedMailingSender updated = CachedMailingSender.FromSender(sender);
        for (int i = 0; i < senders.Length; i++)
        {
            if (updated.Email != senders[i].Email)
                continue;
            senders[i] = updated;
            break;
        }
        await UpdateArray(senders, database);
    }

    private static async Task AddInArray(string json, IEmailSender sender, IDatabase database)
    {
        CachedMailingSender[] senders = JsonSerializer.Deserialize<CachedMailingSender[]>(json)!;
        CachedMailingSender toAdd = CachedMailingSender.FromSender(sender);
        if (senders.Any(s => s.Email == toAdd.Email))
            return;
        senders = [.. senders, toAdd];
        await UpdateArray(senders, database);
    }

    private static async Task UpdateArray(CachedMailingSender[] senders, IDatabase database)
    {
        string updatedJson = JsonSerializer.Serialize(senders);
        await database.StringSetAsync(Key, updatedJson);
    }
}
