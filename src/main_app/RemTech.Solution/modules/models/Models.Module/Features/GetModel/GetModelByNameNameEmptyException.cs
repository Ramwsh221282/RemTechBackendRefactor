namespace Models.Module.Features.GetModel;

internal sealed class GetModelByNameNameEmptyException : Exception
{
    public GetModelByNameNameEmptyException()
        : base("Невозможно получить модель по названию. Параметр названия пустой.") { }

    public GetModelByNameNameEmptyException(Exception ex)
        : base("Невозможно получить модель по названию. Параметр названия пустой.", ex) { }
}
