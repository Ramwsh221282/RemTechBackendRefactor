using Mailers.Core.MailedMessagesModule;

namespace Mailers.Core.MailersModule;

public sealed record MailerSending(Mailer Mailer, MailedMessage Message);