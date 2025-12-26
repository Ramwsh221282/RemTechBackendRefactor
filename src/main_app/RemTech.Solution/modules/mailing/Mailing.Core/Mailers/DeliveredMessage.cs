using Mailing.Core.Inbox;

namespace Mailing.Core.Mailers;

public sealed record DeliveredMessage(Mailer Mailer, InboxMessage Message);