namespace Identity.Core.SubjectsModule.Domain.Credentials;

public static class SubjectCredentialsValidationModule
{
    public const int MaxLength = 256;
    
    extension(SubjectCredentials credentials)
    {
        public Result<SubjectCredentials> Validated()
        {
            string email = credentials.Email;
            string password = credentials.Password;
            EmailString emailString = EmailString.Create(email);
            
            if (Strings.EmptyOrWhiteSpace(password)) return Validation("Пароль не указан.");
            if (Strings.EmptyOrWhiteSpace(email)) return Validation("Почта не указана.");
            if (emailString.IsValid() == false) return Validation("Невалидный формат почты.");
            if (Strings.GreaterThan(email, MaxLength)) return Validation("Невалидный формат почты.");

            return credentials;
        }
    }
}