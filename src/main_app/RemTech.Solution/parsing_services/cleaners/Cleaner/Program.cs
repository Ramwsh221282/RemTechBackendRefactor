using Cleaner;
using Cleaner.Configuration;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);
CleanerConfiguration configuration = CleanerConfiguration.ResolveByEnvironment();
configuration.Register(builder.Services);
builder.Services.AddSingleton<Serilog.ILogger>(
    new LoggerConfiguration().WriteTo.Console().CreateLogger()
);
builder.Services.AddHostedService<Worker>();
var host = builder.Build();
host.Run();
