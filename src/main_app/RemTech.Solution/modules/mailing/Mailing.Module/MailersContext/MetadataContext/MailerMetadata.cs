using Dapper;
using Mailing.Module.Traits;
using Serilog;

namespace Mailing.Module.MailersContext.MetadataContext;

internal sealed class MailerMetadata
{
    private readonly Guid _id;
    private readonly string _email;
    private readonly string _smtpPassword;

    private MailerMetadata()
    {
    }
}