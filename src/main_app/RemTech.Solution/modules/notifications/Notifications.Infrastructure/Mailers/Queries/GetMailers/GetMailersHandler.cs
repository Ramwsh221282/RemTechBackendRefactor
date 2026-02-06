using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailers;

/// <summary>
/// Обработчик запроса получения множества почтовых ящиков.
/// </summary>
/// <param name="repository">Репозиторий почтовых ящиков.</param>
public sealed class GetMailersHandler(IMailersRepository repository)
	: IQueryHandler<GetMailersQuery, IEnumerable<MailerResponse>>
{
	/// <summary>
	///  Обрабатывает запрос получения множества почтовых ящиков.
	/// </summary>
	/// <param name="query">Запрос на получение множества почтовых ящиков.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Коллекция ответов с данными почтовых ящиков.</returns>
	public async Task<IEnumerable<MailerResponse>> Handle(GetMailersQuery query, CancellationToken ct = default)
	{
		MailersSpecification specification = new();
		Mailer[] mailers = await repository.GetMany(specification, ct);
		return mailers.Select(MailerResponse.Create);
	}
}
