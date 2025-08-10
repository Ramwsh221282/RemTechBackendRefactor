namespace RemTech.Spares.Module.Features.SinkSpare.Exceptions;

internal sealed class SparePersistDuplicateIdException : Exception
{
    public SparePersistDuplicateIdException()
        : base($"Дубликат ID запчасти.") { }

    public SparePersistDuplicateIdException(Exception inner)
        : base($"Дубликат ID запчасти.", inner) { }
}
