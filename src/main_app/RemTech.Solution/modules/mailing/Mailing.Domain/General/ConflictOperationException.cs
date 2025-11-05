namespace Mailing.Domain.General;

public sealed class ConflictOperationException(string message) : OperationException(message);