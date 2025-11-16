namespace Identity.Core.SubjectsModule.Domain.Credentials;

public sealed record SubjectCredentials(string Email, string Password)
{
    public Result<SubjectCredentials> WithOtherPassword(string other)
    {
        return SubjectCredentials.Create(Email, other);
    }

    public Result<SubjectCredentials> WithOtherEmail(string other)
    {
        return SubjectCredentials.Create(other, Password);
    }
}