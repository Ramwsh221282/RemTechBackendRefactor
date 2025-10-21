using Identity.Domain.EmailTickets;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.Storage;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.ConfirmUserEmail;

public sealed class ConfirmUserEmailHandler(
    IEmailConfirmationTicketsStorage tickets,
    IUsersStorage users,
    IIdentityUnitOfWork unitOfWork
) : ICommandHandler<ConfirmUserEmailCommand, Status<IdentityUser>>
{
    public async Task<Status<IdentityUser>> Handle(
        ConfirmUserEmailCommand command,
        CancellationToken ct = default
    )
    {
        EmailConfirmationTicket? ticket = await tickets.Get(command.EmailConfirmationId, ct);
        if (ticket == null)
            return Error.NotFound("Заявка на подтверждение почты не найдена.");

        IdentityUser? user = await users.Get(ticket.Email, ct);
        if (user == null)
            return Error.NotFound("Пользователь не найден.");

        Status confirming = await user.ConfirmEmail(unitOfWork, tickets, ticket, ct);
        if (confirming.IsFailure)
            return confirming.Error;

        return user;
    }
}
