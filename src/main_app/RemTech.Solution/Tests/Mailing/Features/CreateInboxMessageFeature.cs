using Mailing.Presenters.Inbox.CreateInboxMessage;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Mailing.Features;

public sealed class CreateInboxMessageFeature(IServiceProvider sp)
{
    public async Task<Result<CreateInboxMessageResponse>> Invoke(
        string targetEmail,
        string subject,
        string body
    )
    {
        CancellationToken ct = CancellationToken.None;
        CreateInboxMessageRequest request = new(targetEmail, subject, body, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<CreateInboxMessageRequest, CreateInboxMessageResponse> service =
            scope.Resolve<IGateway<CreateInboxMessageRequest, CreateInboxMessageResponse>>();
        return await service.Execute(request);
    }
}