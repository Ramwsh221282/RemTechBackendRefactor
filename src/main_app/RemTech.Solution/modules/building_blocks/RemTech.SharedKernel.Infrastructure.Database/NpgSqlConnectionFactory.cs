using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.Infrastructure.Database;

public sealed class NpgSqlConnectionFactory
{
    private readonly NpgsqlDataSource _dataSource;

    public NpgSqlConnectionFactory(IOptions<NpgSqlOptions> options, ILoggerFactory? loggerFactory)
    {
        NpgsqlDataSourceBuilder builder = new NpgsqlDataSourceBuilder(options.Value.ToConnectionString());
        builder.UseVector();
        builder.EnableParameterLogging();
        
        if (loggerFactory is not null)
        {
            builder.UseLoggerFactory(loggerFactory);
        }
        
        _dataSource = builder.Build();
    }

    public async Task<NpgsqlConnection> Create(CancellationToken ct = default)
    {
        return await _dataSource.OpenConnectionAsync(ct);
    }
}