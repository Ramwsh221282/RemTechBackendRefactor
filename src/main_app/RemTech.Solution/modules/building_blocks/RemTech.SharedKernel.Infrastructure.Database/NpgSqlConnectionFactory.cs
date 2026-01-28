using Microsoft.Extensions.Options;
using Npgsql;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.Infrastructure.Database;

/// <summary>
/// Фабрика для создания подключений к базе данных PostgreSQL.
/// </summary>
public sealed class NpgSqlConnectionFactory
{
	private readonly NpgsqlDataSource _dataSource;

	/// <summary>
	/// Инициализирует новый экземпляр <see cref="NpgSqlConnectionFactory"/>.
	/// </summary>
	/// <param name="options">Опции для настройки подключения к базе данных PostgreSQL.</param>
	public NpgSqlConnectionFactory(IOptions<NpgSqlOptions> options)
	{
		NpgsqlDataSourceBuilder builder = new(options.Value.ToConnectionString());
		builder.UseVector();
		_dataSource = builder.Build();
	}

	/// <summary>
	/// Создает и открывает новое подключение к базе данных PostgreSQL.
	/// </summary>
	/// <param name="ct">Токен отмены для асинхронной операции.</param>
	/// <returns>Открытое подключение к базе данных PostgreSQL.</returns>
	public async Task<NpgsqlConnection> Create(CancellationToken ct = default) =>
		await _dataSource.OpenConnectionAsync(ct);
}
