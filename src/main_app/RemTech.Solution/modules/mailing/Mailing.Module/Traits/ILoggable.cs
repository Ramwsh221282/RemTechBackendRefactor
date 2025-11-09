using Serilog;

namespace Mailing.Module.Traits;

public interface ILoggable
{
    void Log(ILogger logger);
}