using Npgsql;
using Shared.Infrastructure.Module.Cqrs;
using Users.Module.Features.UserPasswordRecovering.Core;

namespace Users.Module.Features.UserPasswordRecovering.Interactor;

internal sealed class UserPasswordRecoveringContext : ICommand
{
    private readonly string? _email;
    private readonly string? _login;
    private IUserRecoveringPassword _userRecovering = null!;

    public UserPasswordRecoveringContext(UserPasswordRecoverRequest request)
    {
        _email = request.Email;
        _login = request.Login;
    }

    public void Attach(IUserRecoveringPassword userRecovering)
    {
        _userRecovering = userRecovering;
    }

    public void Print(out string? email, out string? login)
    {
        email = _email;
        login = _login;
    }

    public void AddTo(NpgsqlCommand command)
    {
        _userRecovering.AddTo(command);
    }
}
