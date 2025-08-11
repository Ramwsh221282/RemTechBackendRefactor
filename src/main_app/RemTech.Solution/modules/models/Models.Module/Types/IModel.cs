namespace Models.Module.Types;

internal interface IModel
{
    Guid Id { get; }
    string Name { get; }
    long Rating { get; }
}
