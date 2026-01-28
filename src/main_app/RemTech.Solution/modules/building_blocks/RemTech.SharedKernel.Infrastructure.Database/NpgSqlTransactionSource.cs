using Npgsql;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Infrastructure.Database;

/// <summary>
/// Реализация источника транзакций для Npgsql.
/// </summary>
/// <param name="session">Сессия для работы с базой данных PostgreSQL.</param>
public sealed class NpgSqlTransactionSource(NpgSqlSession session) : ITransactionSource
{
	/// <summary>
	/// Начинает новую транзакцию и возвращает область транзакции.
	/// </summary>
	/// <param name="ct">Токен отмены для асинхронной операции.</param>
	/// <returns>Область транзакции.</returns>
	public async Task<ITransactionScope> BeginTransaction(CancellationToken ct = default)
	{
		NpgsqlTransaction transaction = await session.GetTransaction(ct);
		return new NpgSqlTransactionScope(transaction);
	}
}
