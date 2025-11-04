namespace Mailing.Domain.Postmans;

public sealed record MessageDeliveryContext(string Service, string From, string To, string Subject, string Body);