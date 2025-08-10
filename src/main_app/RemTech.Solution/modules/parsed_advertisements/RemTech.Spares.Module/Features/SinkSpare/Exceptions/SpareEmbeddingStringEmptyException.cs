namespace RemTech.Spares.Module.Features.SinkSpare.Exceptions;

internal sealed class SpareEmbeddingStringEmptyException : Exception
{
    public SpareEmbeddingStringEmptyException()
        : base("Строка для создания эмбеддинга была пустой.") { }

    public SpareEmbeddingStringEmptyException(Exception inner)
        : base("Строка для создания эмбеддинга была пустой.", inner) { }
}
