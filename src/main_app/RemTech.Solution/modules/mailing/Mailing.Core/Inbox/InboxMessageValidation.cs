using Mailing.Core.Common;

namespace Mailing.Core.Inbox;

public static class InboxMessageValidation
{
    extension(InboxMessage message)
    {
        public InboxMessage Validated()
        {
            message.Subject.Validate();
            message.Body.Validate();
            message.TargetEmail.Validate();
            return message;
        }
    }
}