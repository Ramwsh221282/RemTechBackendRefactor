namespace Identity.Domain.Contracts.Outbox;

/// <summary>
/// Спецификация для фильтрации сообщений исходящей очереди.
/// </summary>
public sealed class OutboxMessageSpecification
{
	/// <summary>
	/// Тип сообщения.
	/// </summary>
	public string? Type { get; private set; }

	/// <summary>
	/// Дата и время создания сообщения.
	/// </summary>
	public DateTime? CreatedDateTime { get; private set; }

	/// <summary>
	/// Дата и время отправки сообщения.
	/// </summary>
	public DateTime? SentDateTime { get; private set; }

	/// <summary>
	/// Флаг, указывающий на выбор только отправленных сообщений.
	/// </summary>
	public bool? SentOnly { get; private set; }

	/// <summary>
	/// Флаг, указывающий на выбор только неотправленных сообщений.
	/// </summary>
	public bool? NotSentOnly { get; private set; }

	/// <summary>
	/// Флаг, указывающий на необходимость блокировки сообщений при выборке.
	/// </summary>
	public bool? WithLock { get; private set; }

	/// <summary>
	/// Лимит количества сообщений для выборки.
	/// </summary>
	public int? Limit { get; private set; }

	/// <summary>
	///    Максимальное количество попыток отправки сообщения.
	/// </summary>
	public int? RetryCountLessThan { get; private set; }

	/// <summary>
	/// Создает спецификацию с указанным лимитом.
	/// </summary>
	/// <param name="limit">Лимит количества сообщений для выборки.</param>
	/// <returns>Спецификация с указанным лимитом.</returns>
	public OutboxMessageSpecification OfLimit(int limit)
	{
		if (Limit.HasValue)
		{
			return this;
		}

		Limit = limit;
		return this;
	}

	/// <summary>
	/// Создает спецификацию с максимальным количеством попыток отправки сообщения.
	/// </summary>
	/// <param name="retryCount">Максимальное количество попыток отправки сообщения.</param>
	/// <returns>Спецификация с указанным максимальным количеством попыток отправки сообщения.</returns>
	public OutboxMessageSpecification OfRetryCountLessThan(int retryCount)
	{
		if (RetryCountLessThan.HasValue)
		{
			return this;
		}

		RetryCountLessThan = retryCount;
		return this;
	}

	/// <summary>
	/// Создает спецификацию с указанным типом сообщения.
	/// </summary>
	/// <param name="type">Тип сообщения.</param>
	/// <returns>Спецификация с указанным типом сообщения.</returns>
	public OutboxMessageSpecification OfType(string type)
	{
		if (!string.IsNullOrWhiteSpace(Type))
		{
			return this;
		}

		Type = type;
		return this;
	}

	/// <summary>
	/// Создает спецификацию с указанной датой и временем создания сообщения.
	/// </summary>
	/// <param name="dateTime">Дата и время создания сообщения.</param>
	/// <returns>Спецификация с указанной датой и временем создания сообщения.</returns>
	public OutboxMessageSpecification OfCreatedDateTime(DateTime dateTime)
	{
		if (CreatedDateTime.HasValue)
		{
			return this;
		}

		CreatedDateTime = dateTime;
		return this;
	}

	/// <summary>
	/// Создает спецификацию с указанной датой и временем отправки сообщения.
	/// </summary>
	/// <param name="dateTime">Дата и время отправки сообщения.</param>
	/// <returns>Спецификация с указанной датой и временем отправки сообщения.</returns>
	public OutboxMessageSpecification OfSentDateTime(DateTime dateTime)
	{
		if (SentDateTime.HasValue)
		{
			return this;
		}

		SentDateTime = dateTime;
		return this;
	}

	/// <summary>
	/// Создает спецификацию для выбора только отправленных сообщений.
	/// </summary>
	/// <returns>Спецификация для выбора только отправленных сообщений.</returns>
	public OutboxMessageSpecification OfSentOnly()
	{
		if (SentOnly.HasValue)
		{
			return this;
		}

		SentOnly = true;
		return this;
	}

	/// <summary>
	/// Создает спецификацию для выбора только неотправленных сообщений.
	/// </summary>
	/// <returns>Спецификация для выбора только неотправленных сообщений.</returns>
	public OutboxMessageSpecification OfNotSentOnly()
	{
		if (NotSentOnly.HasValue)
		{
			return this;
		}

		NotSentOnly = true;
		return this;
	}

	/// <summary>
	/// Создает спецификацию с блокировкой сообщений при выборке.
	/// </summary>
	/// <returns>Спецификация с блокировкой сообщений при выборке.</returns>
	public OutboxMessageSpecification OfWithLock()
	{
		if (WithLock.HasValue)
		{
			return this;
		}

		WithLock = true;
		return this;
	}
}
