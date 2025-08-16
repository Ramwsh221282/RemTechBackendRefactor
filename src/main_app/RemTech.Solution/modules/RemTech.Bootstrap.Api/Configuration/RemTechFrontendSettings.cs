namespace RemTech.Bootstrap.Api.Configuration;

public sealed class RemTechFrontendSettings
{
    private const string Key = ConfigurationConstants.FRONTEND_URL_KEY;
    private readonly string _frontendUrl;

    private RemTechFrontendSettings(string frontendUrl) => _frontendUrl = frontendUrl;

    public void ConfigureCors(WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
            options.AddPolicy(
                "FRONTEND",
                conf =>
                    conf.WithOrigins(_frontendUrl)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("Authorization_Token_Id", "Authorization_Token_Value")
            )
        );
    }

    public static RemTechFrontendSettings FromJson(string json)
    {
        IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile(json).Build();
        string? url = root.GetSection(Key).Value;
        return FromValue(url);
    }

    public static RemTechFrontendSettings FromEnv()
    {
        string? url = Environment.GetEnvironmentVariable(Key);
        return FromValue(url);
    }

    private static RemTechFrontendSettings FromValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ApplicationException($"{Key} is not provided.")
            : new RemTechFrontendSettings(value);
    }
}
