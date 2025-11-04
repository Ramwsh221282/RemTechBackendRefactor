namespace Mailing.Domain.Postmans.Factories;

public sealed record PostmanConstructionContext(Guid Id, string SmtpPassword, string Email);