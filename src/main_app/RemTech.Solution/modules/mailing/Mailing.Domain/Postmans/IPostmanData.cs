namespace Mailing.Domain.Postmans;

public interface IPostmanData
{
    Guid Id { get; }
    string Email { get; }
    string SmtpPassword { get; }
}

public sealed record PostmanData(Guid Id, string Email, string SmtpPassword) : IPostmanData;