namespace RemTech.Spares.Module.Features.SinkSpare.Exceptions;

internal sealed class SparePersistanceCommandEmptyValueException : Exception
{
    public SparePersistanceCommandEmptyValueException(string propertyName)
        : base($"Не удается добавить параметр в команду хранения. {propertyName} было пустым.") { }

    public SparePersistanceCommandEmptyValueException(string propertyName, Exception inner)
        : base(
            $"Не удается добавить параметр в команду хранения. {propertyName} было пустым.",
            inner
        ) { }
}
