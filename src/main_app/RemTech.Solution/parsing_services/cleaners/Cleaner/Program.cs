using Cleaner;
using Cleaner.Configuration;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<Serilog.ILogger>(
    new LoggerConfiguration().WriteTo.Console().CreateLogger()
);
if (builder.Environment.IsDevelopment())
{
    CleanerConfiguration configuration = CleanerConfiguration.FromJson("appsettings.json");
    configuration.Register(builder.Services);
}

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
