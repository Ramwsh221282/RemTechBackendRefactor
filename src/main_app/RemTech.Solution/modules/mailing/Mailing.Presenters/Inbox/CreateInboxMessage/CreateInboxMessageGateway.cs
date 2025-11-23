using Mailing.Application;
using Mailing.Application.Inbox.CreateInboxMessage;
using Mailing.Core.Inbox;
using Mailing.Presenters.Shared;
using RemTech.Functional.Extensions;

namespace Mailing.Presenters.Inbox.CreateInboxMessage;

public sealed class CreateInboxMessageGateway
(ICommandHandler<CreateInboxMessageCommand, InboxMessage> handler)
    : IGateway<CreateInboxMessageRequest, CreateInboxMessageResponse>
{
    public async Task<Result<CreateInboxMessageResponse>> Execute(CreateInboxMessageRequest request)
    { 
        CreateInboxMessageCommand command = new(request.TargetEmail, request.Subject, request.Body, request.Ct);
        AsyncOperationResult<InboxMessage> result = new(() => handler.Execute(command));
        Result<InboxMessage> useCaseResult = await result.Process();
        return useCaseResult.Map(v => new CreateInboxMessageResponse(v.TargetEmail.Value, v.Subject.Value, v.Body.Value));
    }
}