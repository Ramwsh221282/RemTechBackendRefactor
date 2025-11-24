using Mailing.Application.Mailers.UpdateMailer;
using Mailing.Core.Mailers;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Mailing.Presenters.Mailers.UpdateMailer;

public record UpdateMailerRequest(
    Guid MailerId, 
    string NewEmail, 
    string NewSmtpPassword, 
    CancellationToken Ct) : IRequest;

public record UpdateMailerResponse(Guid Id) : IResponse;

public sealed class UpdateMailerGateway
(ICommandHandler<UpdateMailerCommand, Mailer> handler)
    : IGateway<UpdateMailerRequest, UpdateMailerResponse>
{
    public async Task<Result<UpdateMailerResponse>> Execute(UpdateMailerRequest request)
    {
        UpdateMailerCommand command = new(request.MailerId, request.NewEmail, request.NewSmtpPassword, request.Ct);
        AsyncOperationResult<Mailer> commandResult = new(() => handler.Execute(command));
        Result<Mailer> result = await commandResult.Process();
        return result.Map(r => new UpdateMailerResponse(r.Id));
    }
}