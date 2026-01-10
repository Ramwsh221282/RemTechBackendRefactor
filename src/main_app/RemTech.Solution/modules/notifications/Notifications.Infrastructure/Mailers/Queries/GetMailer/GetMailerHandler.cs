using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Contracts;
using Notifications.Infrastructure.Mailers.Queries.GetMailers;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailer;

public sealed class GetMailerHandler(IMailersRepository repository)
    : IQueryHandler<GetMailerQuery, MailerResponse?>
{
    public async Task<MailerResponse?> Handle(
        GetMailerQuery query,
        CancellationToken ct = default)
    {
        MailersSpecification spec = new MailersSpecification().WithId(query.Id);
        Result<Mailer> mailer = await repository.Get(spec, ct);
        return mailer.IsSuccess ? MailerResponse.Create(mailer.Value) : null;
    }
}