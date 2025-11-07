namespace RemTech.Core.Shared.Exceptions;

public sealed class ConflictOperationException(string message) : OperationExceptionV2(message);