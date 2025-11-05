namespace Mailing.Domain.General;

public sealed class InvalidObjectStateException(string message) : OperationException(message);