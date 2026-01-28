using Spares.Domain.Models;

namespace Spares.Domain.Contracts;

/// <summary>
/// Репозиторий для работы с запчастями.
/// </summary>
public interface ISparesRepository
{
	/// <summary>
	/// Добавляет несколько запчастей.
	/// </summary>
	/// <param name="spares">Коллекция запчастей.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Количество добавленных элементов.</returns>
	Task<int> AddMany(IEnumerable<Spare> spares, CancellationToken ct = default);
}
