using Mailing.Module.Contracts;
using Mailing.Module.Models;

namespace Mailing.Module.Cache;

internal sealed record CachedMailingSender(string Email, string Name, string Key)
{
    public static CachedMailingSender FromSender(IEmailSender sender)
    {
        EmailSenderOutput output = sender.Print();
        return new CachedMailingSender(output.Email, output.Name, output.Key);
    }

    public IEmailSender AsSender()
    {
        return new EmailSender(Email, Key);
    }
}
