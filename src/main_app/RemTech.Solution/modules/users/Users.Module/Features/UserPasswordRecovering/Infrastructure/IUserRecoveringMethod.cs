using Users.Module.Features.UserPasswordRecovering.Exceptions;

namespace Users.Module.Features.UserPasswordRecovering.Infrastructure;

internal interface IUserRecoveringMethod
{
    Task<PasswordResetMessageDetails> Invoke();
}
