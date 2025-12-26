using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using RemTech.Shared.Configuration;

namespace RemTech.Bootstrap.Api.Extensions;

public static class WebHostExtensions
{
    public static void ConfigureWebHost(
        this WebApplicationBuilder builder,
        int httpPort,
        int grpcPort
    )
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Listen(
                IPAddress.Any,
                httpPort,
                opts =>
                {
                    opts.Protocols = HttpProtocols.Http1AndHttp2;
                }
            );
            options.Listen(
                IPAddress.Any,
                grpcPort,
                opts =>
                {
                    opts.Protocols = HttpProtocols.Http2;
                }
            );
        });
    }

    public static void AddConfigurations(this WebApplicationBuilder builder)
    {
        var configBuilder = new ConfigurationPartsBuilder(builder.Services);
        configBuilder.AddConfigurations();
    }
}
