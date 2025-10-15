using System.Net;
using Mailing.Domain.CommonContext.ValueObjects;
using RemTech.Result.Pattern;

namespace Mailing.Domain.EmailShipperContext.ValueObjects;

public sealed record EmailShipperServiceAddress
{
    private const StringComparison Comparison = StringComparison.OrdinalIgnoreCase;
    public EmailAddress Address { get; }
    public string SmtpHost { get; }

    private EmailShipperServiceAddress(EmailAddress address, string smtpHost)
    {
        Address = address;
        SmtpHost = smtpHost;
    }

    public void Subscribe(EmailShippmentProcess shippment)
    {
        shippment.SmtpClient.Host = SmtpHost;
        shippment.SmtpClient.Credentials = new NetworkCredential(Address.Value, string.Empty);
    }

    public static Result<EmailShipperServiceAddress> Create(EmailAddress address)
    {
        string addressString = address.Value;

        if (addressString.EndsWith("mail.ru", Comparison))
            return new EmailShipperServiceAddress(address, "smtp.mail.ru");

        if (addressString.EndsWith("gmail.com", Comparison))
            return new EmailShipperServiceAddress(address, "smtp.gmail.com");

        if (addressString.EndsWith("yandex.ru", Comparison))
            return new EmailShipperServiceAddress(address, "smtp.yandex.ru");

        return Error.Validation($"Сервис отправки - {addressString} не поддерживается.");
    }
}
