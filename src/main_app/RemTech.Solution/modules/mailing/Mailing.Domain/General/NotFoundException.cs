namespace Mailing.Domain.General;

public sealed class NotFoundException(string message) : Exception;