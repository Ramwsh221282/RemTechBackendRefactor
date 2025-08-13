namespace RemTech.Bootstrap.Api.Configuration;

public static class CorsConfiguring
{
    public static void ConfigureCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
            options.AddPolicy(
                "FRONTEND",
                conf =>
                    conf.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("Authorization_Token_Id", "Authorization_Token_Value")
            )
        );
    }
}
