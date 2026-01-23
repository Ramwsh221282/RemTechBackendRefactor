using Microsoft.Extensions.Options;
using Npgsql;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.Infrastructure.Database;

public sealed class NpgSqlConnectionFactory
{
	private readonly NpgsqlDataSource _dataSource;

	public NpgSqlConnectionFactory(IOptions<NpgSqlOptions> options)
	{
		NpgsqlDataSourceBuilder builder = new(options.Value.ToConnectionString());
		builder.UseVector();
		_dataSource = builder.Build();
	}

	public async Task<NpgsqlConnection> Create(CancellationToken ct = default) =>
		await _dataSource.OpenConnectionAsync(ct);
}
