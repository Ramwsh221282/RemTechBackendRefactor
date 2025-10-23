using Identity.Messaging.Port.EmailTickets;

namespace Identity.Domain.Users.Aggregate;

public static class UserExtensions
{
    public static EmailConfirmationTicket FormTicket(
        this IdentityUser user,
        DateTime created,
        DateTime expires
    )
    {
        Guid ticketId = Guid.NewGuid();
        Guid userId = user.Id.Id;
        string subject = "Подтверждение электронной почты на Ремтехника аггрегатор.";
        string message = """
                         Была подана заявка на подтверждение почты.
                         Для подтверждения почты необходимо перейти по ссылке:
                         <a href="LINK_PLACE">Подтверждение почты</a>
                         """;

        return new EmailConfirmationTicket(ticketId, userId, created, expires, subject, message);
    }
}