using RemTech.NpgSql.Abstractions;

namespace RemTech.Outbox.Shared;

public sealed class OutboxServicesRegistry
{
    private readonly Dictionary<string, Func<string, NpgSqlSession, OutboxService>> _registry = [];

    public void AddService(string dbSchema)
    {
        if (_registry.ContainsKey(dbSchema))
            return;
        _registry.Add(dbSchema, (schema, factory) => new OutboxService(factory, schema));
    }

    public OutboxService GetService(NpgSqlSession session, string dbSchema)
    {
        if (!_registry.ContainsKey(dbSchema))
            throw new KeyNotFoundException(
                $"Outbox service for schema: {dbSchema} was not added in {nameof(OutboxServicesRegistry)}");
        Func<string, NpgSqlSession, OutboxService> factory = _registry[dbSchema];
        return factory(dbSchema, session);
    }
}