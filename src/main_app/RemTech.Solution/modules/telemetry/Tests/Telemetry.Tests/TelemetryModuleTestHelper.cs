using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RemTech.DependencyInjection;
using Remtech.Infrastructure.RabbitMQ;
using Remtech.Infrastructure.RabbitMQ.Consumers;
using RemTech.Result.Pattern;
using RemTech.UseCases.Shared.Cqrs;
using Telemetry.Contracts;
using Telemetry.Domain.TelemetryContext;
using Telemetry.Domain.TelemetryContext.Contracts;
using Telemetry.Infrastructure.RabbitMQ;
using Telemetry.UseCases.SaveActionInfo;

namespace Telemetry.Tests;

public sealed class TelemetryModuleTestHelper
{
    private readonly IServiceProvider _serviceProvider;

    public TelemetryModuleTestHelper(TestApplicationFactory factory) =>
        _serviceProvider = factory.Services;

    public async Task<Result<TelemetryRecord>> CreateRecord(
        string name,
        string status,
        Guid invokerId,
        IEnumerable<string> parts,
        DateTime? occuredAt = null
    )
    {
        var command = new SaveActionInfoIbCommand(parts, name, status, invokerId, occuredAt);
        await using var scope = _serviceProvider.CreateAsyncScope();
        var handler = scope.ServiceProvider.GetRequiredService<
            ICommandHandler<SaveActionInfoIbCommand, TelemetryRecord>
        >();
        return await handler.Handle(command);
    }

    public async Task<IEnumerable<TelemetryRecord>> GetByName(string name)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var repository = scope.GetService<ITelemetryRecordsRepository>();
        return await repository.GetByName(name);
    }

    public async Task<Result<string>> CreateRecordFromRabbitMq(
        string name,
        string status,
        Guid invokerId,
        IEnumerable<string> parts,
        DateTime? occuredAt = null
    )
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        RabbitMqOptions options = scope.GetService<RabbitMqOptions>();
        ConnectionFactory factory = new ConnectionFactory()
        {
            HostName = options.Hostname,
            Port = int.Parse(options.Port),
            UserName = options.Username,
            Password = options.Password,
        };

        await using IConnection connection = await factory.CreateConnectionAsync();
        await using IChannel channel = await connection.CreateChannelAsync();

        SaveActionInfoEvent @event = new SaveActionInfoEvent(
            parts,
            name,
            status,
            invokerId,
            occuredAt
        );
        ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

        await channel.BasicPublishAsync(
            ActionInvokedEventListener.ExchangeName,
            ActionInvokedEventListener.QueueName,
            body: body
        );

        return @event.Name;
    }

    public async Task<Result<TelemetryRecord>> GetById(Guid id)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var repository = scope.GetService<ITelemetryRecordsRepository>();
        return await repository.GetById(id);
    }
}
