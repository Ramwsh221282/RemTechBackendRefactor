using Mailers.Core.EmailsModule;

namespace Mailers.Core.MailedMessagesModule;

public sealed record MailedMessageDeliveryInfo(Email To, DateTime SentOn);