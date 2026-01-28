using System.Data;
using System.Data.Common;
using Dapper;
using Npgsql;

namespace RemTech.SharedKernel.Infrastructure.Database;

/// <summary>
/// Сессия для работы с базой данных PostgreSQL.
/// </summary>
/// <param name="connectionFactory">Фабрика для создания подключений к базе данных PostgreSQL.</param>
public sealed class NpgSqlSession(NpgSqlConnectionFactory connectionFactory) : IDisposable, IAsyncDisposable
{
	private NpgsqlConnection? _connection;

	/// <summary>
	/// Текущая транзакция базы данных, если она существует.
	/// </summary>
	public NpgsqlTransaction? Transaction { get; private set; }

	/// <summary>
	/// Получает подключение к базе данных PostgreSQL, создавая его при необходимости.
	/// </summary>
	/// <param name="ct">Токен отмены для асинхронной операции.</param>
	/// <returns>Подключение к базе данных PostgreSQL.</returns>
	public async Task<NpgsqlConnection> GetConnection(CancellationToken ct)
	{
		if (_connection is not null)
			return _connection;
		_connection = await connectionFactory.Create(ct);
		return _connection;
	}

	/// <summary>
	/// Получает транзакцию базы данных PostgreSQL, создавая ее при необходимости.
	/// </summary>
	/// <param name="ct">Токен отмены для асинхронной операции.</param>
	/// <returns>Транзакция базы данных PostgreSQL.</returns>
	public async Task<NpgsqlTransaction> GetTransaction(CancellationToken ct)
	{
		if (Transaction is not null)
			return Transaction;
		NpgsqlConnection connection = await GetConnection(ct);
		Transaction = await connection.BeginTransactionAsync(ct);
		return Transaction;
	}

	/// <summary>
	/// Формирует команду для выполнения в базе данных PostgreSQL.
	/// </summary>
	/// <param name="sql">SQL-запрос для выполнения.</param>
	/// <param name="parameters">Параметры для SQL-запроса.</param>
	/// <param name="ct">Токен отмены для асинхронной операции.</param>
	/// <returns>Определение команды для выполнения.</returns>
	public CommandDefinition FormCommand(string sql, object parameters, CancellationToken ct) =>
		new(sql, parameters, transaction: Transaction, cancellationToken: ct);

	/// <summary>
	/// Формирует команду для выполнения в базе данных PostgreSQL.
	/// </summary>
	/// <param name="sql">SQL-запрос для выполнения.</param>
	/// <param name="parameters">Параметры для SQL-запроса.</param>
	/// <param name="ct">Токен отмены для асинхронной операции.</param>
	/// <returns>Определение команды для выполнения.</returns>
	public CommandDefinition FormCommand(string sql, DynamicParameters parameters, CancellationToken ct) =>
		new(sql, parameters, transaction: Transaction, cancellationToken: ct);

	/// <summary>
	/// Выполняет команду и возвращает количество затронутых строк.
	/// </summary>
	/// <param name="command">Определение команды для выполнения.</param>
	/// <param name="ct">Токен отмены для асинхронной операции.</param>
	/// <returns>Количество затронутых строк.</returns>
	public async Task<int> WithAffectedCallback(CommandDefinition command, CancellationToken ct)
	{
		NpgsqlConnection connection = await GetConnection(ct);
		return await connection.ExecuteAsync(command);
	}

	/// <summary>
	/// Выполняет команду в базе данных PostgreSQL.
	/// </summary>
	/// <param name="command">Определение команды для выполнения.</param>
	/// <returns>Task.</returns>
	public async Task Execute(CommandDefinition command)
	{
		NpgsqlConnection connection = await GetConnection(CancellationToken.None);
		await connection.ExecuteAsync(command);
	}

	/// <summary>
	/// Выполняет команду в базе данных PostgreSQL.
	/// </summary>
	/// <param name="parameters">Параметры для SQL-запроса.</param>
	/// <param name="sql">SQL-запрос для выполнения.</param>
	/// <param name="ct">Токен отмены для асинхронной операции.</param>
	/// <returns>Task.</returns>
	public async Task Execute(DynamicParameters parameters, string sql, CancellationToken ct)
	{
		NpgsqlTransaction transaction = await GetTransaction(ct);
		NpgsqlConnection connection = await GetConnection(CancellationToken.None);
		CommandDefinition command = new(sql, parameters, transaction: transaction, cancellationToken: ct);
		await connection.ExecuteAsync(command);
	}

	/// <summary>
	/// Считает количество затронутых строк при выполнении команды.
	/// </summary>
	/// <param name="command">Определение команды для выполнения.</param>
	/// <returns>Количество затронутых строк.</returns>
	public async Task<int> CountAffected(CommandDefinition command)
	{
		NpgsqlConnection connection = await GetConnection(CancellationToken.None);
		return await connection.ExecuteAsync(command);
	}

	/// <summary>
	/// Выполняет команду и возвращает одну строку результата.
	/// </summary>
	/// <typeparam name="T">Тип, в который преобразуется строка из запроса.</typeparam>
	/// <param name="command">Определение команды для выполнения.</param>
	/// <returns>Одна строка результата.</returns>
	public async Task<T> QuerySingleRow<T>(CommandDefinition command)
	{
		NpgsqlConnection connection = await GetConnection(CancellationToken.None);
		return await connection.QuerySingleAsync<T>(command);
	}

	/// <summary>
	/// Выполняет команду и возвращает одну строку результата или null, если строка не найдена.
	/// </summary>
	/// <typeparam name="T">Тип, в который преобразуется строка из запроса.</typeparam>
	/// <param name="command">Определение команды для выполнения.</param>
	/// <returns>Одна строка результата или null, если строка не найдена.</returns>
	public async Task<T?> QueryMaybeRow<T>(CommandDefinition command)
	{
		NpgsqlConnection connection = await GetConnection(CancellationToken.None);
		return await connection.QueryFirstOrDefaultAsync<T>(command);
	}

	/// <summary>
	/// Выполняет команду и возвращает несколько строк результата.
	/// </summary>
	/// <typeparam name="T">Тип, в который преобразуются строки из запроса.</typeparam>
	/// <param name="command">Определение команды для выполнения.</param>
	/// <returns>Несколько строк результата.</returns>
	public async Task<IEnumerable<T>> QueryMultipleRows<T>(CommandDefinition command)
	{
		NpgsqlConnection connection = await GetConnection(CancellationToken.None);
		return await connection.QueryAsync<T>(command);
	}

	/// <summary>
	/// Асинхронно освобождает ресурсы, используемые сессией.
	/// </summary>
	/// <returns>Задача, представляющая асинхронную операцию освобождения ресурсов.</returns>
	public async ValueTask DisposeAsync()
	{
		if (Transaction != null)
			await Transaction.DisposeAsync();
		if (_connection != null)
			await _connection.DisposeAsync();
	}

	/// <summary>
	/// Выполняет команду и возвращает одну строку результата, используя IDataReader.
	/// </summary>
	/// <typeparam name="T">Тип, в который преобразуется строка из запроса.</typeparam>
	/// <param name="command">Определение команды для выполнения.</param>
	/// <param name="mapper">Функция для преобразования строки из IDataReader в объект типа T.</param>
	/// <returns>Одна строка результата или null, если строка не найдена.</returns>
	public async Task<T?> QuerySingleUsingReader<T>(CommandDefinition command, Func<IDataReader, T> mapper)
		where T : notnull
	{
		NpgsqlConnection connection = await GetConnection(CancellationToken.None);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		List<T> result = [];
		while (await reader.ReadAsync())
			result.Add(mapper(reader));
		return result.Count == 0 ? default : result[0];
	}

	/// <summary>
	/// Выполняет команду и возвращает основную сущность и связанные сущности, используя IDataReader.
	/// </summary>
	/// <typeparam name="T">Тип основной сущности.</typeparam>
	/// <typeparam name="U">Тип связанной сущности.</typeparam>
	/// <param name="command">Определение команды для выполнения.</param>
	/// <param name="mainEntityMapper">Функция для преобразования строки из IDataReader в основную сущность типа T.</param>
	/// <param name="relatedEntityMapper">Функция для преобразования строки из IDataReader в связанную сущность типа U.</param>
	/// <param name="comparer">Компаратор для сравнения основных сущностей типа T.</param>
	/// <returns>Кортеж, содержащий основную сущность и список связанных сущностей.</returns>
	public async Task<(T? mainEntity, List<U> relatedEntities)> QuerySingleUsingReader<T, U>(
		CommandDefinition command,
		Func<IDataReader, T> mainEntityMapper,
		Func<IDataReader, U?> relatedEntityMapper,
		IEqualityComparer<T> comparer
	)
	{
		NpgsqlConnection connection = await GetConnection(CancellationToken.None);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		Dictionary<T, List<U>> mappings = new(comparer);
		while (await reader.ReadAsync())
		{
			T mainEntity = mainEntityMapper(reader);

			if (!mappings.ContainsKey(mainEntity))
				mappings.Add(mainEntity, []);

			U? related = relatedEntityMapper(reader);
			if (related != null)
				mappings[mainEntity].Add(related);
		}

		return mappings.Count == 0 ? default : (mappings.First().Key, mappings[mappings.First().Key]);
	}

	/// <summary>
	/// Выполняет команду и возвращает несколько строк результата, используя IDataReader.
	/// </summary>
	/// <typeparam name="T">Тип, в который преобразуются строки из запроса.</typeparam>
	/// <param name="command">Определение команды для выполнения.</param>
	/// <param name="mapper">Функция для преобразования строки из IDataReader в объект типа T.</param>
	/// <returns>Массив объектов типа T, представляющих строки результата.</returns>
	public async Task<T[]> QueryMultipleUsingReader<T>(CommandDefinition command, Func<IDataReader, T> mapper)
		where T : notnull
	{
		NpgsqlConnection connection = await GetConnection(CancellationToken.None);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		List<T> result = [];
		while (await reader.ReadAsync())
			result.Add(mapper(reader));
		return [.. result];
	}

	/// <summary>
	/// Выполняет команду и возвращает несколько строк результата, используя IDataReader и указанный компаратор.
	/// </summary>
	/// <typeparam name="T">Тип, в который преобразуются строки из запроса.</typeparam>
	/// <param name="command">Определение команды для выполнения.</param>
	/// <param name="mapper">Функция для преобразования строки из IDataReader в объект типа T.</param>
	/// <param name="comparer">Компаратор для сравнения объектов типа T.</param>
	/// <param name="ct">Токен отмены для асинхронной операции.</param>
	/// <returns>Массив объектов типа T, представляющих строки результата.</returns>
	public async Task<T[]> QueryMultipleUsingReader<T>(
		CommandDefinition command,
		Func<IDataReader, T> mapper,
		IEqualityComparer<T> comparer,
		CancellationToken ct = default
	)
	{
		HashSet<T> results = new(comparer);
		NpgsqlConnection connection = await GetConnection(ct);
		await using DbDataReader reader = await connection.ExecuteReaderAsync(command);
		while (await reader.ReadAsync(ct))
		{
			T item = mapper(reader);
			results.Add(item);
		}

		return [.. results];
	}

	/// <summary>
	/// Выполняет команду в базе данных PostgreSQL.
	/// </summary>
	/// <param name="sql">SQL-запрос для выполнения.</param>
	/// <param name="parameters">Параметры для SQL-запроса.</param>
	/// <returns>Task.</returns>
	public async Task ExecuteBulk(string sql, object[] parameters)
	{
		NpgsqlConnection connection = await GetConnection(CancellationToken.None);
		await connection.ExecuteAsync(sql, parameters);
	}

	/// <summary>
	/// Выполняет команду и возвращает количество затронутых строк.
	/// </summary>
	/// <param name="sql">SQL-запрос для выполнения.</param>
	/// <param name="parameters">Параметры для SQL-запроса.</param>
	/// <returns>Количество затронутых строк.</returns>
	public async Task<int> ExecuteBulkWithAffectedCount(string sql, object[] parameters)
	{
		NpgsqlConnection connection = await GetConnection(CancellationToken.None);
		return await connection.ExecuteAsync(sql, parameters);
	}

	/// <summary>
	/// Освобождает ресурсы, используемые сессией.
	/// </summary>
	public void Dispose()
	{
		Transaction?.Dispose();
		_connection?.Dispose();
	}
}
