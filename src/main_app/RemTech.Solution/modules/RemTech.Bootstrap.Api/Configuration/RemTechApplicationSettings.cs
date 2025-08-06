namespace RemTech.Bootstrap.Api.Configuration;

public sealed class RemTechApplicationSettings
{
    public RemTechDatabaseSettings Database { get; }
    public RemTechCacheSettings Cache { get; }
    public RemTechRabbitMqSettings RabbitMq { get; }

    private RemTechApplicationSettings(
        RemTechDatabaseSettings database,
        RemTechCacheSettings cache,
        RemTechRabbitMqSettings rabbit
    )
    {
        Database = database;
        Cache = cache;
        RabbitMq = rabbit;
    }

    public static RemTechApplicationSettings CreateFromJson(string json)
    {
        RemTechDatabaseSettings database = RemTechDatabaseSettings.CreateFromJson(json);
        RemTechCacheSettings cache = RemTechCacheSettings.CreateFromJson(json);
        RemTechRabbitMqSettings rabbit = RemTechRabbitMqSettings.CreateFromJson(json);
        return new RemTechApplicationSettings(database, cache, rabbit);
    }
}
