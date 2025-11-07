using System.Text.Json;
using Mailing.Module.Domain.Models.ValueObjects;
using Shared.Infrastructure.Module.Redis;
using StackExchange.Redis;

namespace Mailing.Module.Infrastructure.Redis.Models;

internal sealed class RedisMailer
{
    // {
    //  "id": string,
    //  "email": string,
    //  "password": string,
    //  "sendLimit": string,
    //  "currentSend": string
    // }

    private const string KeyTemplate = "MAILER_{0}";
    private const string ArrayTemplate = "MAILERS_ARRAY";
    private readonly Dictionary<string, object> _properties = [];
    private readonly IDatabase _database;
    private readonly TimeSpan _ttl;
    private readonly string _key;
    private readonly Lazy<string> _json;

    public RedisMailer(IDatabase database, IMailer mailer, TimeSpan lifeTime)
    {
        _database = database;
        _json = new(() => JsonSerializer.Serialize(_properties));
        RedisMetadata meta = new(mailer);
        RedisStatistics stats = new(mailer);
        _properties = meta.WriteTo(_properties);
        _properties = stats.WriteTo(_properties);
        _key = meta.SignKey(KeyTemplate);
        _ttl = lifeTime;
    }

    public RedisMailer(RedisCache cache, IMailer mailer, TimeSpan lifeTime) : this(cache.Database, mailer, lifeTime)
    {
    }

    private string Json => _json.Value;


    public async Task Save()
    {
        await _database.StringSetAsync(_key, Json, _ttl);
        Mailer[] mailers = await GetMailers();
        Mailer current = Deserialize(_json.Value);
        bool hasUpdated = false;
        for (int i = 0; i < mailers.Length; i++)
        {
            if (mailers[i].EqualById(current))
            {
                mailers[i] = current;
                hasUpdated = true;
                break;
            }
        }

        if (!hasUpdated)
            mailers = [current, ..mailers];
        await UpdateArray(mailers);
    }

    public async Task Delete()
    {
        Mailer[] mailers = await GetMailers();
        Mailer current = Deserialize(_json.Value);
        mailers = [..mailers.Where(m => !m.EqualById(current))];
        await UpdateArray(mailers);
        await _database.KeyDeleteAsync(_key);
    }

    private async Task<Mailer[]> GetMailers()
    {
        string? jsons = await _database.StringGetAsync(ArrayTemplate);
        return string.IsNullOrEmpty(jsons) ? [] : DeserializeArray(jsons);
    }

    private async Task UpdateArray(Mailer[] mailers)
    {
        List<string> jsons = [];
        foreach (Mailer mailer in mailers)
        {
            string json = new RedisMailer(_database, mailer, _ttl)._json.Value;
            jsons.Add(json);
        }

        await _database.StringSetAsync(ArrayTemplate, JsonSerializer.Serialize(jsons), _ttl);
    }

    private Mailer[] DeserializeArray(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;
        if (root.ValueKind != JsonValueKind.Array)
            return [];

        int i = 0;
        Mailer[] mailers = new Mailer[root.GetArrayLength()];
        foreach (JsonElement element in root.EnumerateArray())
        {
            mailers[i] = Deserialize(element.GetRawText());
            i++;
        }

        return mailers;
    }

    private Mailer Deserialize(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement rootElement = document.RootElement;
        return new Mailer(
            new Metadata(
                rootElement.GetProperty("id").GetGuid(),
                rootElement.GetProperty("email").GetString()!,
                rootElement.GetProperty("password").GetString()!),
            new Statistics(
                rootElement.GetProperty("sendLimit").GetInt32(),
                rootElement.GetProperty("currentSend").GetInt32()));
    }
}