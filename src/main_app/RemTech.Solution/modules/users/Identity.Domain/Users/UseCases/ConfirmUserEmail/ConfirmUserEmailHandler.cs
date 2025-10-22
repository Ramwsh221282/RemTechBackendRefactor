using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.DomainEvents;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.ConfirmUserEmail;

public sealed class ConfirmUserEmailHandler(
    IUsersStorage users,
    IDomainEventsDispatcher eventsHandler
) : ICommandHandler<ConfirmUserEmailCommand, Status<IdentityUser>>
{
    public async Task<Status<IdentityUser>> Handle(
        ConfirmUserEmailCommand command,
        CancellationToken ct = default
    )
    {
        IdentityTokenId id = IdentityTokenId.Create(command.TicketId);
        IdentityUser? user = await users.Get(id, ct);
        if (user == null)
            return Error.NotFound("Пользователь с таким токеном не найден.");

        Status confirmation = user.ConfirmEmail(id);
        if (confirmation.IsFailure)
            return confirmation.Error;

        Status handling = await user.PublishEvents(eventsHandler, ct);
        return handling.IsFailure ? handling.Error : user;
    }
}
