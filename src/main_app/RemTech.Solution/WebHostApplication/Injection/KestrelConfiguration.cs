using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace WebHostApplication.Injection;

public static class KestrelConfiguration
{
    extension(WebApplicationBuilder builder)
    {
        public void ConfigureKestrelServer()
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(
                    IPAddress.Any,
                    5185,
                    opts =>
                    {
                        opts.Protocols = HttpProtocols.Http1AndHttp2;
                    }
                );
                options.Listen(
                    IPAddress.Any,
                    5238,
                    opts =>
                    {
                        opts.Protocols = HttpProtocols.Http2;
                    }
                );
            });
        }
    }
}