namespace Users.Module.Features.RegisteringUser.Exceptions;

internal sealed class UserRegistrationValidationFailedException : Exception
{
    public UserRegistrationValidationFailedException(string error, Exception inner)
        : base(error, inner) { }

    public UserRegistrationValidationFailedException(string error)
        : base(error) { }
}
