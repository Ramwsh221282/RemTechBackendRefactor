using System.Net;
using Mailing.Domain.CommonContext.ValueObjects;

namespace Mailing.Domain.EmailShipperContext.ValueObjects;

public sealed record ShipperEmailShippmentProcessSubscription(
    EmailShipperServiceAddress Address,
    EmailShipperKey Key
)
{
    private const int SmtpPort = 587;

    public void Subscribe(EmailShippmentProcess process)
    {
        process.SmtpClient.Port = SmtpPort;
        process.SmtpClient.Host = Address.SmtpHost;
        NetworkCredential credentials = new(Address.Address.Value, Key.Value);
        process.SmtpClient.Credentials = credentials;
        process.SmtpClient.EnableSsl = true;
    }
}
