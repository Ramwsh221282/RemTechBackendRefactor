namespace Identity.Core.SubjectsModule.Domain.Metadata;

public sealed record SubjectMetadata(Guid Id, string Login)
{
    public const int MAX_LOGIN_LENGTH = 128;
}