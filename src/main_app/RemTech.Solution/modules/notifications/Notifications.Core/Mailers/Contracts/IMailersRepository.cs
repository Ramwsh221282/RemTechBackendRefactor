using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Notifications.Core.Mailers.Contracts;

/// <summary>
/// Контракт репозитория почтовых рассылок.
/// </summary>
public interface IMailersRepository
{
	/// <summary>
	/// Добавляет новую почтовую рассылку в репозиторий.
	/// </summary>
	/// <param name="mailer">Почтовая рассылка для добавления.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию добавления.</returns>
	Task Add(Mailer mailer, CancellationToken ct = default);

	/// <summary>
	/// Получает почтовую рассылку из репозитория по заданной спецификации.
	/// </summary>
	/// <param name="specification">Спецификация для фильтрации почтовых рассылок.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию получения.</returns>
	Task<Result<Mailer>> Get(MailersSpecification specification, CancellationToken ct = default);

	/// <summary>
	/// Получает множество почтовых рассылок из репозитория по заданной спецификации.
	/// </summary>
	/// <param name="specification">Спецификация для фильтрации почтовых рассылок.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию получения множества почтовых рассылок.</returns>
	Task<Mailer[]> GetMany(MailersSpecification specification, CancellationToken ct = default);

	/// <summary>
	/// Проверяет существование почтовой рассылки в репозитории по заданной спецификации.
	/// </summary>
	/// <param name="specification">Спецификация для фильтрации почтовых рассылок.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию проверки существования.</returns>
	Task<bool> Exists(MailersSpecification specification, CancellationToken ct = default);

	/// <summary>
	/// Удаляет почтовую рассылку из репозитория.
	/// </summary>
	/// <param name="mailer">Почтовая рассылка для удаления.</param>
	/// <param name="ct">Токен отмены.</param>
	/// <returns>Задача, представляющая асинхронную операцию удаления.</returns>
	Task Delete(Mailer mailer, CancellationToken ct = default);
}
