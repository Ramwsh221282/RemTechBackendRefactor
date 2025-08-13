namespace RemTech.Bootstrap.Api.Configuration;

public sealed class RemTechApplicationSettings
{
    public RemTechDatabaseSettings Database { get; }
    public RemTechRabbitMqSettings RabbitMq { get; }
    public RemTechCacheSettings Cache { get; }

    private RemTechApplicationSettings(
        RemTechDatabaseSettings database,
        RemTechRabbitMqSettings rabbit,
        RemTechCacheSettings cache
    )
    {
        Database = database;
        RabbitMq = rabbit;
        Cache = cache;
    }

    public static RemTechApplicationSettings CreateFromJson(string json)
    {
        RemTechDatabaseSettings database = RemTechDatabaseSettings.CreateFromJson(json);
        RemTechRabbitMqSettings rabbit = RemTechRabbitMqSettings.CreateFromJson(json);
        RemTechCacheSettings cache = RemTechCacheSettings.CreateFromJson(json);
        return new RemTechApplicationSettings(database, rabbit, cache);
    }
}
