using Mailing.Core.Common;
using Mailing.Core.Inbox;
using Mailing.Core.Inbox.Protocols;
using RemTech.SharedKernel.Core.Handlers;
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
        InboxMessage validated = message.Validated();
        await validated.Save(protocol, args.Ct);
        logger.Information("Pending inbox message created.");
        return validated;
    }
}