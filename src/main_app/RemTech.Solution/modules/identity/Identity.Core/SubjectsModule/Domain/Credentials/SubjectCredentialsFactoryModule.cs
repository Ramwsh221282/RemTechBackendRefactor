namespace Identity.Core.SubjectsModule.Domain.Credentials;

public static class SubjectCredentialsFactoryModule
{
    extension(SubjectCredentials)
    {
        public static Result<SubjectCredentials> Create(string email, string password)
        {
            return new SubjectCredentials(email, password).Validated();
        }
    }
}