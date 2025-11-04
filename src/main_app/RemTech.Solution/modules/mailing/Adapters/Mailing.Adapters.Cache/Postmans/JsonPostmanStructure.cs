using Mailing.Domain.Postmans;

namespace Mailing.Adapters.Cache.Postmans;

internal sealed class JsonPostmanStructure
{
    public required Guid Id { get; init; }
    public required string Email { get; init; }
    public required string SmtpPassword { get; init; }

    public IPostman Cached()
    {
        PostmanData data = new(Id, Email, SmtpPassword);
        Postman postman = new Postman(data);
        return postman;
    }
}