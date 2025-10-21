using Identity.Domain.EmailTickets;
using Identity.Domain.EmailTickets.Ports;
using Identity.Domain.Mailing.Ports;
using Identity.Domain.Users.ValueObjects;
using RemTech.Core.Shared.Result;

namespace Identity.Domain.Mailing;

public sealed record IdentityMailingMessage
{
    public string Subject { get; }
    public string Body { get; }

    private IdentityMailingMessage(string subject, string body) =>
        (Subject, Body) = (subject, body);

    public async Task<Status> Send(
        IIdentityEmailSender sender,
        UserEmail to,
        CancellationToken ct = default
    ) => await sender.SendEmail(this, to, ct);

    public static IdentityMailingMessage AddressConfirmationMessage(
        IFrontendUrlProvider provider,
        EmailConfirmationTicket ticket
    )
    {
        const string template = "{0}/email-confirmation?confirmationKey={1}";
        string frontendUrlString = provider.Provide();
        string confirmationAddress = string.Format(template, frontendUrlString, ticket.Id);
        string subject = "Подтверждение почты RemTech агрегатор спецтехники.";
        string body = $"""
            Была подана заявка на подтверждение почты.
            Для подтверждения почты необходимо перейти по ссылке:
            <a href="{confirmationAddress}">Подтверждение почты</a>
            """;
        return new IdentityMailingMessage(subject, body);
    }
}
