using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Contracts;
using Notifications.Infrastructure.Mailers.Queries.GetMailers;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailer;

/// <summary>
/// Обработчик запроса получения почтового ящика по идентификатору.
/// </summary>
/// <param name="repository">Репозиторий почтовых ящиков.</param>
public sealed class GetMailerHandler(IMailersRepository repository) : IQueryHandler<GetMailerQuery, MailerResponse?>
{
	/// <summary>
	///   Обрабатывает запрос получения почтового ящика по идентификатору.
	/// </summary>
	/// <param name="query">Запрос на получение почтового ящика.</param>
	/// <param name="ct">Токен отмены операции.</param>
	/// <returns>Ответ с данными почтового ящика или null, если почтовый ящик не найден.</returns>
	public async Task<MailerResponse?> Handle(GetMailerQuery query, CancellationToken ct = default)
	{
		MailersSpecification spec = new MailersSpecification().WithId(query.Id);
		Result<Mailer> mailer = await repository.Read(spec, ct);
		return mailer.IsSuccess ? MailerResponse.Create(mailer.Value) : null;
	}
}
