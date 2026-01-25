using Serilog.Core;
using Serilog.Events;

namespace RemTech.SharedKernel.Core.Logging;

public sealed class ClassNameLogEnricher : ILogEventEnricher
{
    private const string Pattern = "SourceContext";

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.TryGetValue(Pattern, out LogEventPropertyValue? sourceContext))
        {
            string fullName = sourceContext.ToString().Trim('\"');
            string exactTypeName = fullName.Split('.')[^1];
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(Pattern, exactTypeName));
        }
    }
}
