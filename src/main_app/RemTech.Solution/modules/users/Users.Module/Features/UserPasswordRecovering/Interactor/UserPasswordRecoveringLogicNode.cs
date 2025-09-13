using Mailing.Module.Bus;
using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Shared.Infrastructure.Module.Frontend;
using StackExchange.Redis;
using Users.Module.Features.UserPasswordRecovering.Core;
using Users.Module.Features.UserPasswordRecovering.Infrastructure;

namespace Users.Module.Features.UserPasswordRecovering.Interactor;

internal sealed class UserPasswordRecoveringLogicNode
    : ICommandHandler<UserPasswordRecoveringContext>
{
    private readonly NpgsqlDataSource _dataSource;
    private readonly ConnectionMultiplexer _multiplexer;
    private readonly FrontendUrl _frontendUrl;
    private readonly MailingBusPublisher _publisher;

    public UserPasswordRecoveringLogicNode(
        NpgsqlDataSource dataSource,
        ConnectionMultiplexer multiplexer,
        FrontendUrl frontendUrl,
        MailingBusPublisher publisher
    )
    {
        _dataSource = dataSource;
        _multiplexer = multiplexer;
        _frontendUrl = frontendUrl;
        _publisher = publisher;
    }

    public async Task Handle(UserPasswordRecoveringContext command, CancellationToken ct = default)
    {
        await using NpgsqlConnection connection = await _dataSource.OpenConnectionAsync(ct);
        await using NpgsqlCommand sqlCommand = connection.CreateCommand();
        command.AddTo(sqlCommand);
        UserToRecover user = await UserToRecover.Create(sqlCommand, ct);
        IUserRecoveringMethod method = user.DecideMethod(_multiplexer, _frontendUrl, _publisher);
        await method.Invoke();
    }
}
