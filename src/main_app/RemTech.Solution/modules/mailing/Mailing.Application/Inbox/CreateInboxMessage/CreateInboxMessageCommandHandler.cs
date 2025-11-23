using Mailing.Core.Common;
using Mailing.Core.Inbox;
using Mailing.Core.Inbox.Protocols;
using Serilog;

namespace Mailing.Application.Inbox.CreateInboxMessage;

public sealed class CreateInboxMessageCommandHandler(
    ILogger logger,
    SaveInboxMessageProtocol protocol) 
    : ICommandHandler<CreateInboxMessageCommand, InboxMessage>
{
    public async Task<InboxMessage> Execute(CreateInboxMessageCommand args)
    {
        logger.Information("Creating pending inbox message.");
        Email email = new(args.TargetEmail);
        MessageSubject subject = new(args.Subject);
        MessageBody body = new(args.Body);
        InboxMessage message = new InboxMessage(Guid.NewGuid(), email, subject, body).Validated();
        await message.Save(protocol, args.Ct);
        logger.Information("Pending inbox message created.");
        return message;
    }
}