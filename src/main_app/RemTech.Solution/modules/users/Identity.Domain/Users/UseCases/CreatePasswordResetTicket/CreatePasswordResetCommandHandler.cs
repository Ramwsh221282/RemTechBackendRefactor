using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.UseCases.Common;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.CreatePasswordResetTicket;

public sealed class CreatePasswordResetCommandHandler(
    IGetUserByEmailHandle byEmail,
    IGetUserByLoginHandle byLogin,
    IDomainEventsDispatcher dispatcher
) : ICommandHandler<CreatePasswordResetCommand, Status<User>>
{
    public async Task<Status<User>> Handle(
        CreatePasswordResetCommand command,
        CancellationToken ct = default
    )
    {
        var user = await byEmail.Handle(command.IssuerEmail, ct);
        if (user.IsFailure)
            user = await byLogin.Handle(command.IssuerLogin, ct);

        if (user.IsFailure)
            return user.Error;

        Status creatingTicket = user.Value.FormResetPasswordTicket();
        if (creatingTicket.IsFailure)
            return creatingTicket.Error;

        Status handling = await user.Value.PublishEvents(dispatcher, ct);
        return handling.IsFailure ? handling.Error : user;
    }
}
