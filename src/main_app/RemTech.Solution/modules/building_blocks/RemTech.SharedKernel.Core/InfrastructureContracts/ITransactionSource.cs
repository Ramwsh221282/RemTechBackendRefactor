namespace RemTech.SharedKernel.Core.InfrastructureContracts;

/// <summary>
/// Интерфейс для источника транзакций.
/// </summary>
public interface ITransactionSource
{
	/// <summary>
	/// Начинает новую транзакцию.
	/// </summary>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Объект, представляющий транзакционную область.</returns>
	Task<ITransactionScope> BeginTransaction(CancellationToken ct = default);
}
