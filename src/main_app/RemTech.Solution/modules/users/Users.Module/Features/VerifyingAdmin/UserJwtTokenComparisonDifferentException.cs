namespace Users.Module.Features.VerifyingAdmin;

internal sealed class UserJwtTokenComparisonDifferentException : Exception
{
    public UserJwtTokenComparisonDifferentException()
        : base("Not origin jwt token.") { }
}
