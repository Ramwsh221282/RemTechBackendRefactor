using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Entities.Tickets.ValueObjects;

public sealed record UserTicketLifeTime
{
    public DateTime Created { get; }

    public DateTime Expires { get; }
    public DateTime? Deleted { get; }

    public bool IsAlive() => Deleted == null && Expires > DateTime.UtcNow;

    public static Status<UserTicketLifeTime> Create(
        DateTime created,
        DateTime expires,
        DateTime? deleted = null
    )
    {
        if (IsDateInvalid(created))
            return Error.Validation("Дата создания пользовательского токена невалидна.");

        if (IsDateInvalid(expires))
            return Error.Validation("Дата окончания заявки невалидна.");

        if (deleted != null)
            if (IsDateInvalid(deleted.Value))
                return Error.Validation("Дата удаления пользовательского токена невалидна.");

        if (created > deleted)
            return Error.Validation(
                "Пользовательская заявка не может иметь дату создания больше даты удаления."
            );

        if (created > expires)
            return Error.Validation(
                "Пользовательская заявка не может иметь дату создания больше даты истечения."
            );

        return new UserTicketLifeTime(created, expires, deleted);
    }

    private UserTicketLifeTime(DateTime created, DateTime expired, DateTime? deleted) =>
        (Created, Expires, Deleted) = (created, expired, deleted);

    private static bool IsDateInvalid(DateTime date) =>
        date == DateTime.MinValue || date == DateTime.MaxValue;
}
