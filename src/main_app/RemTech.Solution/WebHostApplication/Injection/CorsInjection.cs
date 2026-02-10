using RemTech.SharedKernel.Configurations;

namespace WebHostApplication.Injection;

public static class CorsInjection
{
    extension(WebApplicationBuilder builder)
    {
        public void RegisterCors(Serilog.ILogger logger)
        {
            builder.Services.AddCors(options =>
            {
                IConfigurationSection section = builder.Configuration.GetSection(nameof(FrontendOptions));
                string url = section["Url"] ?? throw new InvalidOperationException("Frontend URL option is empty.");
                logger.Information("Регистрация CORS политики для фронтенда с URL: {Url}", url);
        
                options.AddPolicy("frontend", policy =>
                {
                    policy
                        .WithOrigins(
                            url,
                            url + ":80",
                            "http://localhost",
                            "http://localhost:80",
                            "http://frontend:80"
                        )
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        }
    }
}