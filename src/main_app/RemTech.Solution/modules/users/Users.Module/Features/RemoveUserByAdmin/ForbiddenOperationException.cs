namespace Users.Module.Features.RemoveUserByAdmin;

internal sealed class ForbiddenOperationException : Exception
{
    public ForbiddenOperationException()
        : base("Операция запрещена.") { }
}
