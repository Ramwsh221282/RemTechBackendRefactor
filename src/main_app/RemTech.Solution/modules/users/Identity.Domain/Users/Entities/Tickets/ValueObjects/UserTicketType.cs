using RemTech.Core.Shared.Result;

namespace Identity.Domain.Users.Entities.Tickets.ValueObjects;

public sealed record UserTicketType
{
    public static UserTicketType EmailConfirmation = new("Email_Confirmation");
    public static UserTicketType PasswordChangeConfirmation = new("Password_Change_Confirmation");
    public static UserTicketType SessionRemoveConfirmation = new UserTicketType(
        "Session_Remove_Confirmation"
    );

    public const int MaxLength = 100;
    public string Value { get; }

    public UserTicketType(UserTicketType ticket) => Value = ticket.Value;

    private UserTicketType(string value) => Value = value;

    public static Status<UserTicketType> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Error.Validation("Тип токена был пустым.");

        if (value.Length > MaxLength)
            return Error.Validation("Тип токена невалиден.");

        return new UserTicketType(value);
    }
}
