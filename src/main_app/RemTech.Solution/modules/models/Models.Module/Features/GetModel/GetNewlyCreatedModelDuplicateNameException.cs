namespace Models.Module.Features.GetModel;

internal sealed class GetNewlyCreatedModelDuplicateNameException : Exception
{
    public GetNewlyCreatedModelDuplicateNameException(string name)
        : base($"Дубликат модели по названию {name}") { }

    public GetNewlyCreatedModelDuplicateNameException(string name, Exception inner)
        : base($"Дубликат модели по названию {name}", inner) { }
}
