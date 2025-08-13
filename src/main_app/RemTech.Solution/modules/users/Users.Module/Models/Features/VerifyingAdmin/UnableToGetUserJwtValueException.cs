namespace Users.Module.Models.Features.VerifyingAdmin;

internal sealed class UnableToGetUserJwtValueException : Exception
{
    public UnableToGetUserJwtValueException()
        : base("Unable to find user jwt session.") { }
}
