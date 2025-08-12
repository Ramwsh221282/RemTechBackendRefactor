using Scalar.AspNetCore;

namespace RemTech.Bootstrap.Api.Configuration;

public static class MiddlewaresRegister
{
    public static void RegisterMiddlewares(this WebApplication app)
    {
        app.UseCors("FRONTEND");
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
        app.UseHttpsRedirection();
        app.RegisterEndpoints();
    }
}
