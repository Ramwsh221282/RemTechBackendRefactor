using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public sealed class RabbitMqConnectionSource(Serilog.ILogger logger, IOptions<RabbitMqOptions> options)
{
    private IConnection? Connection { get; set; }
    private RabbitMqOptions Options { get; } = options.Value;
    private SemaphoreSlim Semaphore { get; } = new(1);

    private Serilog.ILogger Logger { get; } = logger.ForContext<RabbitMqConnectionSource>();

    public ValueTask<IConnection> GetConnection(CancellationToken ct = default)
    {
        Logger.Debug("Getting connection.");
        Options.Validate();
        if (Connection is not null && Connection.IsOpen)
        {
            Logger.Information("Connection is initialized and open. Returning existing instance.");
            return new ValueTask<IConnection>(Connection);
        }

        async ValueTask<IConnection> Create(CancellationToken ct)
        {
            Logger.Information("Creating connection. Blocking until semaphore is available.");
            await Semaphore.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                ConnectionFactory factory = new()
                {
                    HostName = Options.Hostname,
                    Password = Options.Password,
                    Port = Options.Port,
                    UserName = Options.Username,
                };

                IConnection connection = await factory.CreateConnectionAsync(cancellationToken: ct);
                Connection = connection;
                Logger.Information("Connection created and stored.");
                return connection;
            }
            finally
            {
                Logger.Debug("Semaphore released.");
                Semaphore.Release();
            }
        }

        return Create(ct);
    }
}
