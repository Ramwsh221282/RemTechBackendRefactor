using System.Text.Json;
using Mailing.Domain.General;
using Mailing.Domain.Postmans;
using Mailing.Domain.Postmans.Factories;
using Mailing.Domain.Postmans.Storing;
using StackExchange.Redis;

namespace Mailing.Adapters.Cache.Postmans;

internal sealed class JsonPostman(IDatabase database, string? json = null)
    : IPostmanMetadataStorage, IPostmanStatisticsStorage
{
    private const string KeyTemplate = "POSTMAN_JSON_ENTITY_{0}";
    private readonly string _json = json ?? string.Empty;
    private Guid _id = Guid.Empty;
    private string _email = string.Empty;
    private string _password = string.Empty;
    private int _limitSend;
    private int _currentSend;

    public async Task Save()
    {
        TimeSpan lifeTime = TimeSpan.FromMinutes(10);
        string serialized = Serialized();
        await database.StringSetAsync(GenerateKey(), serialized, lifeTime);
    }

    public async Task<bool> HasUniqueEmail()
    {
        string? json = await database.StringGetAsync(GenerateKey());
        return json == null;
    }

    public void Save(Guid id, string email, string password) =>
        (_id, _email, _password) = (id, email, password);

    public void Save(int sendLimit, int currentSend) =>
        (_currentSend, _limitSend) = (sendLimit, currentSend);

    private string GenerateKey() => string.Format(KeyTemplate, _email);

    private string Serialized()
    {
        if (!string.IsNullOrWhiteSpace(_json)) return _json;
        Dictionary<string, object> data = new()
        {
            { "id", _id },
            { "email", _email },
            { "password", _password },
            { "limit_send", _limitSend },
            { "current_send", _currentSend }
        };
        return JsonSerializer.Serialize(data);
    }

    private IPostman Deserialized(IPostmansFactory factory)
    {
        Dictionary<string, object>? data = JsonSerializer.Deserialize<Dictionary<string, object>>(_json);
        if (data == null) throw new ConflictOperationException("Некорректный json Postman");
        Guid id = (Guid)data["id"];
        string email = (string)data["email"];
        string password = (string)data["password"];
        int limitSend = (int)data["limit_send"];
        int currentSend = (int)data["current_send"];
        return factory.Construct(id, email, password, limitSend, currentSend);
    }
}