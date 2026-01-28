using Npgsql;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace RemTech.SharedKernel.Infrastructure.Database;

/// <summary>
/// Реализация области транзакции для Npgsql.
/// </summary>
/// <param name="transaction">Транзакция Npgsql для управления областью транзакции.</param>
public sealed class NpgSqlTransactionScope(NpgsqlTransaction transaction) : ITransactionScope
{
	private NpgsqlTransaction Transaction { get; } = transaction;

	/// <summary>
	/// Фиксирует транзакцию.
	/// </summary>
	/// <param name="ct">Токен отмены для асинхронной операции.</param>
	/// <returns>Результат операции фиксации транзакции.</returns>
	public async Task<Result> Commit(CancellationToken ct = default)
	{
		try
		{
			await Transaction.CommitAsync(ct);
			return Result.Success();
		}
		catch
		{
			await Transaction.RollbackAsync(ct);
			return Result.Failure(Error.Application("Ошибка транзакции."));
		}
	}

	/// <summary>
	/// Освобождает ресурсы, используемые областью транзакции.
	/// </summary>
	public void Dispose() => Transaction.Dispose();

	/// <summary>
	/// Асинхронно освобождает ресурсы, используемые областью транзакции.
	/// </summary>
	/// <returns>ValueTask.</returns>
	public ValueTask DisposeAsync() => Transaction.DisposeAsync();
}
