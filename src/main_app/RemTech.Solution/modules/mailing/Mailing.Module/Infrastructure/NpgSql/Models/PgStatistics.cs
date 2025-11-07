using Dapper;
using System.Data;
using RemTech.Core.Shared.Reflection;

namespace Mailing.Module.Infrastructure.NpgSql.Models;

internal sealed class PgStatistics
{
    private readonly int _sendLimit;
    private readonly int _currentSend;

    public DynamicParameters WriteTo(DynamicParameters? parameters)
    {
        parameters ??= new DynamicParameters();
        parameters.Add("@current_sent", _sendLimit, DbType.Int32);
        parameters.Add("@current_limit", _currentSend, DbType.Int32);
        return parameters;
    }

    public PgStatistics(IMailer mailer)
    {
        FieldsSearcher searcher = new(mailer);
        _sendLimit = searcher.SearchByName<int>(nameof(_sendLimit));
        _currentSend = searcher.SearchByName<int>(nameof(_currentSend));
    }
}