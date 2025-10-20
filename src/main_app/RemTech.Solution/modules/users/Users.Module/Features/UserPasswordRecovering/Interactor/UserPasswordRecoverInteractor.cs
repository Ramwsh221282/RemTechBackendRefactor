using Mailing.Module.Bus;
using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Shared.Infrastructure.Module.Frontend;
using StackExchange.Redis;

namespace Users.Module.Features.UserPasswordRecovering.Interactor;

internal sealed class UserPasswordRecoverInteractor : ICommandHandler<UserPasswordRecoverRequest>
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ConnectionMultiplexer _multiplexer;
    private readonly FrontendUrl _frontendUrl;
    private readonly MailingBusPublisher _publisher;
    private readonly Serilog.ILogger _logger;

    public UserPasswordRecoverInteractor(
        NpgsqlDataSource dataSource,
        ConnectionMultiplexer multiplexer,
        FrontendUrl frontendUrl,
        MailingBusPublisher publisher,
        Serilog.ILogger logger
    )
    {
        _dataSource = dataSource;
        _multiplexer = multiplexer;
        _frontendUrl = frontendUrl;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task Handle(UserPasswordRecoverRequest command, CancellationToken ct = default)
    {
        UserPasswordRecoveringContext context = new(command);
        ICommandHandler<UserPasswordRecoveringContext> handler =
            new UserPasswordExceptionLoggingNode(
                _logger,
                new UserPasswordRecoveringChain()
                    .AddNode(new UserPasswordRecoveringValidationNode())
                    .AddNode(
                        new UserPasswordRecoveringLogicNode(
                            _dataSource,
                            _multiplexer,
                            _frontendUrl,
                            _publisher
                        )
                    )
            );
        await handler.Handle(context, ct);
    }
}
