namespace Models.Module.Types;

internal sealed record Model(Guid Id, string Name, long Rating) : IModel;
