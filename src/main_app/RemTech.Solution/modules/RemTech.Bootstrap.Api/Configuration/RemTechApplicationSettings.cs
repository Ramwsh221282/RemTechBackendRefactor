namespace RemTech.Bootstrap.Api.Configuration;

public sealed class RemTechApplicationSettings
{
    private readonly RemTechFrontendSettings _frontend;
    public RemTechDatabaseSettings Database { get; }
    public RemTechRabbitMqSettings RabbitMq { get; }
    public RemTechCacheSettings Cache { get; }
    public RemTechSeqSettings Seq { get; }

    private RemTechApplicationSettings(
        RemTechDatabaseSettings database,
        RemTechRabbitMqSettings rabbit,
        RemTechCacheSettings cache,
        RemTechSeqSettings seq,
        RemTechFrontendSettings frontend
    )
    {
        Database = database;
        RabbitMq = rabbit;
        Cache = cache;
        Seq = seq;
        _frontend = frontend;
    }

    public static RemTechApplicationSettings CreateFromJson(string json)
    {
        RemTechDatabaseSettings database = RemTechDatabaseSettings.CreateFromJson(json);
        RemTechRabbitMqSettings rabbit = RemTechRabbitMqSettings.CreateFromJson(json);
        RemTechFrontendSettings frontend = RemTechFrontendSettings.FromJson(json);
        RemTechCacheSettings cache = RemTechCacheSettings.CreateFromJson(json);
        RemTechSeqSettings seq = RemTechSeqSettings.CreateFromJson(json);
        return new RemTechApplicationSettings(database, rabbit, cache, seq, frontend);
    }

    public static RemTechApplicationSettings CreateFromEnv()
    {
        RemTechDatabaseSettings db = RemTechDatabaseSettings.FromEnv();
        RemTechRabbitMqSettings rabbit = RemTechRabbitMqSettings.FromEnv();
        RemTechCacheSettings cache = RemTechCacheSettings.FromEnv();
        RemTechSeqSettings seq = RemTechSeqSettings.FromEnv();
        RemTechFrontendSettings frontend = RemTechFrontendSettings.FromEnv();
        return new RemTechApplicationSettings(db, rabbit, cache, seq, frontend);
    }

    public static RemTechApplicationSettings ResolveByEnvironment(WebApplicationBuilder builder)
    {
        try
        {
            return CreateFromJson("appsettings.json");
        }
        catch
        {
            return CreateFromEnv();
        }
    }

    public void ConfigureCors(WebApplicationBuilder app)
    {
        _frontend.ConfigureCors(app);
    }
}
