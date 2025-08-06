namespace RemTech.Bootstrap.Api.Configuration;

public sealed class RemTechApplicationSettings
{
    public RemTechDatabaseSettings Database { get; }
    public RemTechRabbitMqSettings RabbitMq { get; }

    private RemTechApplicationSettings(
        RemTechDatabaseSettings database,
        RemTechRabbitMqSettings rabbit
    )
    {
        Database = database;
        RabbitMq = rabbit;
    }

    public static RemTechApplicationSettings CreateFromJson(string json)
    {
        RemTechDatabaseSettings database = RemTechDatabaseSettings.CreateFromJson(json);
        RemTechRabbitMqSettings rabbit = RemTechRabbitMqSettings.CreateFromJson(json);
        return new RemTechApplicationSettings(database, rabbit);
    }
}
