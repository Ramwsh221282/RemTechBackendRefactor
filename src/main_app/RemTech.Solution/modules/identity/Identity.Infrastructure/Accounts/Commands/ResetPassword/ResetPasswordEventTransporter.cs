using Identity.Domain.Accounts.Features.ResetPassword;
using RemTech.SharedKernel.Core.Handlers.Decorators.DomainEvents;
using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace Identity.Infrastructure.Accounts.Commands.ResetPassword;

public sealed record ResetPasswordRequiredMessage(
    Guid AccountId,
    string AccountEmail,
    Guid TicketId,
    string TicketPurpose
);

public sealed class ResetPasswordEventTransporter(RabbitMqProducer producer, Serilog.ILogger logger)
    : IEventTransporter<ResetPasswordCommand, ResetPasswordResult>
{
    private const string Exchange = "accounts";
    private Serilog.ILogger Logger { get; } = logger.ForContext<ResetPasswordCommand>();
    private RabbitMqProducer Producer { get; } = producer;

    public async Task Transport(ResetPasswordResult result, CancellationToken ct = default)
    {
        ResetPasswordRequiredMessage message = MapToMessage(result);
        await PublishMessageToRabbitMq(message, ct);

        Logger.Information(
            "Published ResetPasswordRequiredMessage to RabbitMQ for AccountId: {AccountId} and Email: {Email}",
            result.AccountId,
            result.AccountEmail
        );
    }

    private static ResetPasswordRequiredMessage MapToMessage(ResetPasswordResult result) =>
        new(
            AccountId: result.AccountId,
            AccountEmail: result.AccountEmail,
            TicketId: result.TicketId,
            TicketPurpose: result.TicketPurpose
        );

    private async Task PublishMessageToRabbitMq(
        ResetPasswordRequiredMessage message,
        CancellationToken ct
    )
    {
        RabbitMqPublishOptions options = new() { Persistent = true };
        await Producer.PublishDirectAsync(message, Exchange, message.TicketPurpose, options, ct);
    }
}
