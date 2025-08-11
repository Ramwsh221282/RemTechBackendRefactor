namespace Models.Module.Features.GetModel;

internal sealed class UnableToResolveModelException : Exception
{
    public UnableToResolveModelException()
        : base("Невозможно разрешить модель.") { }

    public UnableToResolveModelException(Exception ex)
        : base("Невозможно разрешить модель.", ex) { }
}
