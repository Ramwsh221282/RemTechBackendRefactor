namespace Models.Module.Features.GetModel;

internal sealed class GetModelByNameNotFoundException : Exception
{
    public GetModelByNameNotFoundException(string name)
        : base($"Не удается найти модель по названию {name}") { }

    public GetModelByNameNotFoundException(string name, Exception ex)
        : base($"Не удается найти модель по названию {name}", ex) { }
}
