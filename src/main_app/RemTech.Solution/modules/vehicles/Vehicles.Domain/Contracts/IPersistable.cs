using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Vehicles.Domain.Contracts;

/// <summary>
/// Интерфейс для объектов, которые могут быть сохранены с помощью персистера.
/// </summary>
/// <typeparam name="T">Тип объекта, который может быть сохранен.</typeparam>
public interface IPersistable<T>
	where T : class
{
	/// <summary>
	/// Сохраняет объект с помощью указанного персистера.
	/// </summary>
	/// <param name="persister">Персистер для сохранения объекта.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Результат сохранения объекта.</returns>
	Task<Result<T>> SaveBy(IPersister persister, CancellationToken ct = default);
}
