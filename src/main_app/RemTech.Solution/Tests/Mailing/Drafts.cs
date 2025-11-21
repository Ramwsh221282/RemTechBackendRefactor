using Mailing.Core.Abstractions;
using Mailing.Core.Inbox.InboxMessages;

namespace Tests.Mailing;

public sealed class Drafts
{
    [Fact]
    private void Test_Design()
    {
        SyncMessagesChoreography logging = new();
        SyncMessagesChoreography businessLogic = new(logging);
        
        MessagesLogger logger = new(
            errors: new ErroredMessagesLogger(), 
            handled: new HandledMessagesLogger(), 
            choreography: logging);
        
        InboxMessage message = new(
            targetEmail: "target email", 
            subject: "subject", 
            body: "body", 
            choreography: businessLogic);
        
        InboxMessageRegistrationOffice office = new(isClosed: false, choreography: businessLogic);
        InboxMessageStore store = new(choreography: businessLogic);
        
        message.Register();
        
        businessLogic.ProcessMessages();
        logging.ProcessMessages();
    }
}