using Mailing.Core.Common;

namespace Mailing.Core.Inbox;

public sealed record InboxMessage(Guid Id, Email TargetEmail, MessageSubject Subject, MessageBody Body);