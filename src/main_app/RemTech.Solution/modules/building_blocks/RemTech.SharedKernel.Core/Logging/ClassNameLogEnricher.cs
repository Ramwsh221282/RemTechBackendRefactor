using Serilog.Core;
using Serilog.Events;

namespace RemTech.SharedKernel.Core.Logging;

/// <summary>
/// Enricher для логов, добавляющий только имя класса в свойство SourceContext вместо полного имени типа.
/// </summary>
public sealed class ClassNameLogEnricher : ILogEventEnricher
{
	/// <summary>
	/// Шаблон свойства SourceContext.
	/// </summary>
	private const string PATTERN = "SourceContext";

	/// <summary>
	/// Обогащает событие лога, добавляя только имя класса в свойство SourceContext.
	/// </summary>
	/// <param name="logEvent">Событие лога для обогащения.</param>
	/// <param name="propertyFactory">Фабрика для создания свойств лога.</param>
	public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
	{
		if (logEvent.Properties.TryGetValue(PATTERN, out LogEventPropertyValue? sourceContext))
		{
			string fullName = sourceContext.ToString().Trim('\"');
			string exactTypeName = fullName.Split('.')[^1];
			logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty(PATTERN, exactTypeName));
		}
	}
}
