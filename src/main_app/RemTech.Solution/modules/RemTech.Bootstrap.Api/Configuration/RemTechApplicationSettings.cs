namespace RemTech.Bootstrap.Api.Configuration;

public sealed class RemTechApplicationSettings
{
    public RemTechDatabaseSettings Database { get; }
    public RemTechRabbitMqSettings RabbitMq { get; }
    public RemTechCacheSettings Cache { get; }
    public RemTechSeqSettings Seq { get; }

    private RemTechApplicationSettings(
        RemTechDatabaseSettings database,
        RemTechRabbitMqSettings rabbit,
        RemTechCacheSettings cache,
        RemTechSeqSettings seq
    )
    {
        Database = database;
        RabbitMq = rabbit;
        Cache = cache;
        Seq = seq;
    }

    public static RemTechApplicationSettings CreateFromJson(string json)
    {
        RemTechDatabaseSettings database = RemTechDatabaseSettings.CreateFromJson(json);
        RemTechRabbitMqSettings rabbit = RemTechRabbitMqSettings.CreateFromJson(json);
        RemTechCacheSettings cache = RemTechCacheSettings.CreateFromJson(json);
        RemTechSeqSettings seq = RemTechSeqSettings.CreateFromJson(json);
        return new RemTechApplicationSettings(database, rabbit, cache, seq);
    }
}
