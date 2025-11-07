using RemTech.Core.Shared.Reflection;

namespace Mailing.Module.Infrastructure.Redis.Models;

internal sealed class RedisMetadata
{
    private readonly Guid _id;
    private readonly string _email;
    private readonly string _password;

    public string SignKey(string key) => string.Format(key, _email);

    public Dictionary<string, object> WriteTo(Dictionary<string, object> properties)
    {
        properties.Add("id", _id);
        properties.Add("email", _email);
        properties.Add("password", _password);
        return properties;
    }

    public RedisMetadata(IMailer postman)
    {
        FieldsSearcher searcher = new(postman);
        _id = searcher.SearchByName<Guid>(nameof(_id));
        _email = searcher.SearchByName<string>(nameof(_email));
        _password = searcher.SearchByName<string>(nameof(_password));
    }
}