namespace Users.Module.Features.VerifyingAdmin;

internal sealed class UnableToGetUserJwtValueException : Exception
{
    public UnableToGetUserJwtValueException()
        : base("Unable to find user jwt session.") { }
}
