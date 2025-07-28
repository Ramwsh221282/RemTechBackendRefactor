using Serilog;

namespace RemTech.Logging.Adapter;

public sealed class LoggerSource
{
    public ILogger Logger() => new LoggerConfiguration().WriteTo.Console().CreateLogger();
}