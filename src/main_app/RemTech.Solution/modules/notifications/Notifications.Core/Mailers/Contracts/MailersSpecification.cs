namespace Notifications.Core.Mailers.Contracts;

/// <summary>
/// Спецификация для фильтрации почтовых рассылок.
/// </summary>
public sealed class MailersSpecification
{
	/// <summary>
	/// Идентификатор почтовой рассылки.
	/// </summary>
	public Guid? Id { get; private set; }

	/// <summary>
	/// Email почтовой рассылки.
	/// </summary>
	public string? Email { get; private set; }

	/// <summary>
	/// Требуется ли блокировка почтовой рассылки.
	/// </summary>
	public bool? LockRequired { get; private set; }

	/// <summary>
	/// Устанавливает идентификатор почтовой рассылки.
	/// </summary>
	/// <param name="id">Идентификатор почтовой рассылки.</param>
	/// <returns>Объект спецификации с установленным идентификатором.</returns>
	public MailersSpecification WithId(Guid id)
	{
		if (Id.HasValue)
		{
			return this;
		}

		Id = id;
		return this;
	}

	/// <summary>
	/// Устанавливает email почтовой рассылки.
	/// </summary>
	/// <param name="email">Email почтовой рассылки.</param>
	/// <returns>Объект спецификации с установленным email.</returns>
	public MailersSpecification WithEmail(string email)
	{
		if (!string.IsNullOrWhiteSpace(Email))
		{
			return this;
		}

		Email = email;
		return this;
	}

	/// <summary>
	/// Устанавливает требование блокировки почтовой рассылки.
	/// </summary>
	/// <returns>Объект спецификации с установленным требованием блокировки.</returns>
	public MailersSpecification WithLockRequired()
	{
		LockRequired = true;
		return this;
	}
}
