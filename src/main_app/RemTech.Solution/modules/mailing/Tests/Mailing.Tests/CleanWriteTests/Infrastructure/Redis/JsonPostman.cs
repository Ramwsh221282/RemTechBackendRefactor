using System.Text.Json;
using Mailing.Domain.General;
using StackExchange.Redis;

namespace Mailing.Tests.CleanWriteTests.Infrastructure.Redis;

public sealed class JsonPostman
{
    private readonly Dictionary<string, object> _properties = [];
    private const string KeyTemplate = "CACHED_POSTMAN_{0}";
    private string _cachedEmailField = string.Empty;

    public async Task Save(IDatabase database)
    {
        string key = GenerateKey();
        TimeSpan lifeTime = TimeSpan.FromMinutes(10);
        string serialized = Serialized();
        await database.StringSetAsync(key, serialized, lifeTime);
    }


    public void AddId(Guid id) => _properties.Add("id", id);
    public void AddPassword(string password) => _properties.Add("password", password);
    public void AddLimitSend(int sendLimit) => _properties.Add("limit_send", sendLimit.ToString());
    public void AddCurrentSend(int currentSend) => _properties.Add("current_send", currentSend.ToString());

    public void AddEmail(string email)
    {
        _cachedEmailField = email;
        _properties.Add("email", email);
    }

    private string Serialized() => JsonSerializer.Serialize(_properties);

    private string GenerateKey() =>
        string.IsNullOrEmpty(_cachedEmailField)
            ? throw new InvalidObjectStateException("Не удается сохранить Postman в кеше. Email требуется для ключа.")
            : string.Format(KeyTemplate, _cachedEmailField);
}