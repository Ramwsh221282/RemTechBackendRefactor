namespace Mailers.Core.EmailsModule;

public sealed record Email
{
    public string Value { get; init; }
    
    internal Email(string value) => Value = value;

    public Result<Email> Change(string email)
    {
        return new Email(email).Validated();
    }

    public string UserPart()
    {
        return EmailParts()[^0];
    }

    public string DomainPart()
    {
        return EmailParts()[^1];
    }

    private string[] EmailParts()
    {
        return Value.Split('@');
    }
}