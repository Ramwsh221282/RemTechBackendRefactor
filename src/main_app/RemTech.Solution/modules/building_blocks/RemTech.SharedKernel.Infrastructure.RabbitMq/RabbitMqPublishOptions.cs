namespace RemTech.SharedKernel.Infrastructure.RabbitMq;

/// <summary>
/// Опции публикации сообщений в RabbitMQ.
/// </summary>
public sealed class RabbitMqPublishOptions
{
	/// <summary>
	/// Указывает, должны ли сообщения быть постоянными.
	/// </summary>
	public bool Persistent { get; set; } = true;

	/// <summary>
	/// Указывает, должны ли сообщения быть обязательными.
	/// </summary>
	public bool Mandatory { get; set; } = true;
}
