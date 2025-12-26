using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RemTech.SharedKernel.Configurations;

namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

public sealed class RabbitMqConnectionSource(IOptions<RabbitMqOptions> options)
{
    private IConnection? Connection { get; set; }
    private RabbitMqOptions Options { get; } = options.Value;
    private SemaphoreSlim Semaphore { get; } = new(1);

    public ValueTask<IConnection> GetConnection(CancellationToken ct = default)
    {
        Options.Validate();
        if (Connection is not null && Connection.IsOpen) return new ValueTask<IConnection>(Connection);
        
        async ValueTask<IConnection> Create(CancellationToken ct)
        {
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
                return connection;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        return Create(ct);
    }
}