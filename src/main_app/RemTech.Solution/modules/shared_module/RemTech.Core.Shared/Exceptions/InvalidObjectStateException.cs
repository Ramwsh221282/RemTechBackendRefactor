namespace RemTech.Core.Shared.Exceptions;

public sealed class InvalidObjectStateException(string message) : OperationExceptionV2(message);