using Identity.Domain.EmailTickets;
using Identity.Domain.EmailTickets.Ports;
using Identity.Domain.Mailing;
using Identity.Domain.Mailing.Ports;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Cqrs;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.UseCases.CreateEmailConfirmation;

public sealed class CreateEmailConfirmationCommandHandler(
    IEmailConfirmationTicketsStorage tickets,
    IUsersStorage users,
    IFrontendUrlProvider frontend,
    IIdentityEmailSender mailSender
) : ICommandHandler<CreateEmailConfirmationCommand, Status<CreateEmailConfirmationResponse>>
{
    public async Task<Status<CreateEmailConfirmationResponse>> Handle(
        CreateEmailConfirmationCommand command,
        CancellationToken ct = default
    )
    {
        // получить пользователя.
        Status<IdentityUser> user = await FindUser(command, ct);
        if (user.IsFailure)
            return user.Error;

        // сгенерировать тикет для подтверждения почты.
        Status<EmailConfirmationTicket> ticket = await MakeTicket(user, ct);
        if (ticket.IsFailure)
            return ticket.Error;

        // сгенерировать сообщение для отправки подтверждения на почту.
        IdentityMailingMessage message = IdentityMailingMessage.AddressConfirmationMessage(
            frontend,
            ticket
        );

        // отправка сообщения на почту.
        Status sending = await message.Send(mailSender, ticket.Value.Email, ct);
        if (sending.IsFailure)
            return sending.Error;

        return new CreateEmailConfirmationResponse(
            "На почту было отправлено сообщение для подтверждения."
        );
    }

    private async Task<Status<IdentityUser>> FindUser(
        CreateEmailConfirmationCommand command,
        CancellationToken ct
    )
    {
        UserId id = UserId.Create(command.UserId);
        IdentityUser? user = await users.Get(id, ct);
        return user == null ? Error.NotFound("Пользователь не найден.") : user;
    }

    private async Task<Status<EmailConfirmationTicket>> MakeTicket(
        IdentityUser identityUser,
        CancellationToken ct
    ) => await EmailConfirmationTicket.New(identityUser).Save(tickets, ct);
}
