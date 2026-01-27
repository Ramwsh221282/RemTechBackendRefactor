namespace Spares.Domain.Features;

/// <summary>
/// Публикатор событий о добавлении запчастей.
/// </summary>
public interface IOnSparesAddedEventPublisher
{
	/// <summary>
	/// Публикует событие о добавлении запчастей.
	/// </summary>
	/// <param name="creatorId">Идентификатор пользователя, добавившего запчасти.</param>
	/// <param name="addedAmount">Количество добавленных запчастей.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача публикации события.</returns>
	Task Publish(Guid creatorId, int addedAmount, CancellationToken ct = default);
}
