using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace RemTech.SharedKernel.Core.InfrastructureContracts;

/// <summary>
/// Интерфейс для управления транзакционной областью.
/// </summary>
public interface ITransactionScope : IDisposable, IAsyncDisposable
{
	/// <summary>
	/// Фиксирует транзакцию.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Результат выполнения операции фиксации.</returns>
	Task<Result> Commit(CancellationToken ct = default);
}
