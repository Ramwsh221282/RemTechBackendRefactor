namespace Identity.Core.SubjectsModule.Models;

public sealed record IdentitySubjectMetadata(Guid Id, string Login)
{
    public const int MAX_LOGIN_LENGTH = 128;
}