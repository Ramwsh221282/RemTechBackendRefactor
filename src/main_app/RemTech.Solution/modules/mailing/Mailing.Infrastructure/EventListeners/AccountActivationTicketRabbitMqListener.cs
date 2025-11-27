using System.Text;
using System.Text.Json;
using Mailing.Application.Inbox.CreateInboxMessage;
using Mailing.Core.Inbox;
using Mailing.Infrastructure.NpgSql.Inbox;
using RabbitMQ.Client.Events;
using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;
using RemTech.SharedKernel.Infrastructure.NpgSql;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace Mailing.Infrastructure.EventListeners;

public sealed class AccountActivationTicketRabbitMqListener(
    NpgSqlConnectionFactory npgSqlConnectionFactory, 
    Serilog.ILogger logger,
    RabbitMqConnectionSource connectionSource) : RabbitMqListenerHostService(connectionSource)
{
    private const string Queue = "identity";
    private const string Exchange = "identity";
    private const string Type = "topic";
    private const string RoutingKey = "account.activation.ticket.required";
    private const string Context = nameof(AccountActivationTicketRabbitMqListener);
    
    protected override Func<RabbitMqConnectionSource, CancellationToken, Task<RabbitMqListener>> GetListener()
    {
        return async (source, token) =>
        {
            DeclareQueueArgs args = new(Queue, Exchange, RoutingKey, Type);
            AsyncEventHandler<BasicDeliverEventArgs> handler = _handler;
            return await source.CreateListener(_handler, args, token);
        };
    }

    private AsyncEventHandler<BasicDeliverEventArgs> _handler => async (_, @event) =>
    {
        try
        {
            logger.Information("{Context} received event to create inbox message.", Context);
            
            await using NpgSqlSession session = new(npgSqlConnectionFactory);
            NpgSqlSaveInboxMessageProtocol saveMessageProtocol = new(session);
            CreateInboxMessageCommandHandler handler = new(logger, saveMessageProtocol);
            CreateInboxMessageCommand command = MakeCommandFromEvent(@event);
            InboxMessage result = await handler.Execute(command);
            
            logger.Information("""
                               {Context} message has been saved. Save info:
                               Target: {TargetEmail}
                               Subject: {Subject}
                               """, [ Context, result.TargetEmail.Value, result.Subject.Value, ]);
        }
        catch(Exception ex)
        {
            logger.Error("{Context} error: {Error}", Context, ex.Message);
        }
    };

    private static CreateInboxMessageCommand MakeCommandFromEvent(BasicDeliverEventArgs @event)
    {
        string jsonEventBody = Encoding.UTF8.GetString(@event.Body.Span);
        using JsonDocument document = JsonDocument.Parse(jsonEventBody);
        string? ticketId = document.RootElement.GetProperty("ticket_id").GetString();
        string? accountEmail = document.RootElement.GetProperty("account_email").GetString();
        string frontendUrl = "dummy.frontend";
        if (string.IsNullOrWhiteSpace(ticketId))
            throw ErrorException.Conflict("Ticket id was not present.");
        if (string.IsNullOrWhiteSpace(accountEmail))
            throw ErrorException.Conflict("Account email was not present.");
        
        const string subject = "Подтверждение почты учетной записи Remtech агрегатор";
        const string bodyTemplate = """
                                    Необходимо перейти по ссылке для подтверждения учетной записи {0}/{1}
                                    """;
        
        string body = string.Format(bodyTemplate, frontendUrl, ticketId);
        CreateInboxMessageCommand command = new(accountEmail, subject, body);
        return command;
    }
}