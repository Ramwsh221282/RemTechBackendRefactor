using RemTech.Core.Shared.Reflection;

namespace Mailing.Module.Infrastructure.Redis.Models;

internal sealed class RedisStatistics
{
    private readonly int _sendLimit;
    private readonly int _currentSend;

    public Dictionary<string, object> WriteTo(Dictionary<string, object> properties)
    {
        properties.Add("sendLimit", _sendLimit);
        properties.Add("currentSend", _currentSend);
        return properties;
    }

    public RedisStatistics(IMailer mailer)
    {
        FieldsSearcher searcher = new(mailer);
        _sendLimit = searcher.SearchByName<int>(nameof(_sendLimit));
        _currentSend = searcher.SearchByName<int>(nameof(_currentSend));
    }
}