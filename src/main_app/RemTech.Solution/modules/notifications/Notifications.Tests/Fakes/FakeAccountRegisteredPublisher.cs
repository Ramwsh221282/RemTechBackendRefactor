using RemTech.SharedKernel.Infrastructure.RabbitMq;

namespace Notifications.Tests.Fakes;

public sealed class FakeAccountRegisteredPublisher(RabbitMqProducer producer, Serilog.ILogger logger)
{
    private const string Exchange = "identity";
    private const string RoutingKey = "account.new";

    private RabbitMqProducer Producer { get; } = producer;
    private Serilog.ILogger Logger { get; } = logger.ForContext<FakeAccountRegisteredPublisher>();

    public async Task Publish(Guid accountId, Guid ticketId, string email, string login)
    {
        RabbitMqPublishOptions options = new() { Persistent = true };
        NewAccountRegisteredOutboxMessagePayload payload = new(accountId, ticketId, email, login);
        await Producer.PublishDirectAsync(payload, Exchange, RoutingKey, options, CancellationToken.None);
        Logger.Information("Published account registration message for {Email}", email);
    }

    private sealed record NewAccountRegisteredOutboxMessagePayload(
        Guid AccountId,
        Guid TicketId,
        string Email,
        string Login);
}