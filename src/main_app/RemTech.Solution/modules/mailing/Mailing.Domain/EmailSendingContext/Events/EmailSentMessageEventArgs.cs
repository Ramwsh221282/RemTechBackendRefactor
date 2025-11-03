using Mailing.Domain.SendedMessageContext;
using RemTech.Core.Shared.Result;

namespace Mailing.Domain.EmailSendingContext.Events;

public sealed class EmailSentMessageEventArgs
{
    private readonly Guid _id;
    private readonly Guid _senderId;
    private readonly string _subject;
    private readonly string _body;
    private readonly DateTime _created;
    private readonly string _recipientEmail;

    public void Fold(Action<Guid, Guid, string, string, DateTime, string> foldFn) =>
        foldFn(_id, _senderId, _subject, _body, _created, _recipientEmail);

    public T Fold<T>(Func<Guid, Guid, string, string, DateTime, string, T> foldFn) =>
        foldFn(_id, _senderId, _subject, _body, _created, _recipientEmail);

    internal EmailSentMessageEventArgs(SendedMessage message)
        : this(FromMessage(message))
    {
    }

    internal EmailSentMessageEventArgs(
        Guid id,
        Guid senderId,
        string subject,
        string body,
        DateTime created,
        string recipient) =>
        (_id, _senderId, _subject, _body, _created, _recipientEmail) =
        (id, senderId, subject, body, created, recipient);

    internal EmailSentMessageEventArgs(EmailSentMessageEventArgs ea)
        : this(ea._id, ea._senderId, ea._subject, ea._body, ea._created, ea._recipientEmail)
    {
    }

    internal static EmailSentMessageEventArgs FromMessage(SendedMessage message) =>
        from id in message.Id.AsSuccessStatus()
        from sender_id in message.SenderId.Value.AsSuccessStatus()
        from subject in message.Content.Body.AsSuccessStatus()
        from body in message.Content.Body.AsSuccessStatus()
        from date in message.Date.Value.AsSuccessStatus()
        from recipient in message.Content.RecipientEmail.Fold(s => s).AsSuccessStatus()
        select new EmailSentMessageEventArgs(id.Value, sender_id, subject, body, date, recipient);
}