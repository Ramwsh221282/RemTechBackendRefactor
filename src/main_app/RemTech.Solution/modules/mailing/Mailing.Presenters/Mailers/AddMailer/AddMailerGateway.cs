using Mailing.Application.Mailers.AddMailer;
using Mailing.Core.Mailers;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Mailing.Presenters.Mailers.AddMailer;

public sealed class AddMailerGateway 
(ICommandHandler<AddMailerCommand, Mailer> handler)
    : IGateway<AddMailerRequest, AddMailerResponse>
{
    public async Task<Result<AddMailerResponse>> Execute(AddMailerRequest request)
    {
        AddMailerCommand command = new(Email: request.Email, Password: request.Password, request.Ct);
        AsyncOperation<Mailer> result = new(() => handler.Execute(command));
        Result<Mailer> commandResult = await result.Process();
        return commandResult.Map(r => new AddMailerResponse(MailerId: r.Id));
    }
}