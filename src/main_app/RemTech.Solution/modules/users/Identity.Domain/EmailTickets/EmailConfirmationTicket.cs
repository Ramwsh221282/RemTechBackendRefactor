using Identity.Domain.Users;
using Identity.Domain.Users.Aggregate;
using Identity.Domain.Users.Ports.Storage;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.EmailTickets;

public sealed record EmailConfirmationTicket
{
    public Guid Id { get; }
    public DateTime CreatedDate { get; }
    public int MinutesUntilDestroy { get; }
    public UserEmail Email { get; }

    private EmailConfirmationTicket(
        Guid id,
        DateTime createdDate,
        int minutesUntilDestroy,
        UserEmail email
    ) =>
        (Id, CreatedDate, MinutesUntilDestroy, Email) = (
            id,
            createdDate,
            minutesUntilDestroy,
            email
        );

    public async Task<Status<EmailConfirmationTicket>> Save(
        IEmailConfirmationTicketsStorage storage,
        CancellationToken ct = default
    )
    {
        Status adding = await storage.Add(this, ct);
        return adding.IsFailure ? adding.Error : this;
    }

    public static EmailConfirmationTicket New(
        IdentityUser identityUser,
        int minutesUntilDestroy = 10
    )
    {
        Guid id = Guid.NewGuid();
        DateTime createdDate = DateTime.UtcNow;
        UserEmail email = identityUser.Email;
        return new EmailConfirmationTicket(id, createdDate, minutesUntilDestroy, email);
    }
}
