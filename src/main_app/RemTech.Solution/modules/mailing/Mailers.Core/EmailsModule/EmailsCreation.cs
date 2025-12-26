namespace Mailers.Core.EmailsModule;

public static class EmailsCreation
{
    extension(Email)
    {
        public static Result<Email> Construct(string input)
        {
            return new Email(input).Validated();
        }
    }
}