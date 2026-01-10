using Notifications.Core.Mailers;
using Notifications.Core.Mailers.Contracts;
using RemTech.SharedKernel.Core.Handlers;

namespace Notifications.Infrastructure.Mailers.Queries.GetMailers;

public sealed class GetMailersHandler(IMailersRepository repository)
    : IQueryHandler<GetMailersQuery, IEnumerable<MailerResponse>>
{
    public async Task<IEnumerable<MailerResponse>> Handle(GetMailersQuery query, CancellationToken ct = default)
    {
        MailersSpecification specification = new MailersSpecification();
        Mailer[] mailers = await repository.GetMany(specification, ct);
        return mailers.Select(MailerResponse.Create);
    }
}