using Identity.Core.SubjectsModule.Contracts;

namespace Identity.Core.SubjectsModule.Domain.Credentials;

public sealed record SubjectCredentials(string Email, string Password)
{
    public Result<SubjectCredentials> WithOtherPassword(string other)
    {
        return SubjectCredentials.Create(Email, other);
    }
    
    public Result<SubjectCredentials> WithOtherPassword(string other, HashPassword hash)
    {
        Result<SubjectCredentials> withOther = WithOtherPassword(other);
        if (withOther.IsFailure) return withOther.Error;
        string hashed = hash(withOther.Value.Password);
        return this with { Password = hashed };
    }

    public Result<SubjectCredentials> WithOtherEmail(string other)
    {
        return SubjectCredentials.Create(other, Password);
    }
}