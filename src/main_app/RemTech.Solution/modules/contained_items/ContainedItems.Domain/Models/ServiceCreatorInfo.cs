using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

/// <summary>
/// Информация о создателе сервиса.
/// </summary>
public sealed record ServiceCreatorInfo
{
	private const int MAX_LENGTH = 128;

	private ServiceCreatorInfo(Guid creatorId, string type, string domain)
	{
		CreatorId = creatorId;
		Type = type;
		Domain = domain;
	}

	/// <summary>
	/// Идентификатор создателя сервиса.
	/// </summary>
	public Guid CreatorId { get; }

	/// <summary>
	/// Тип сервиса.
	/// </summary>
	public string Type { get; }

	/// <summary>
	/// Домен сервиса.
	/// </summary>
	public string Domain { get; }

	/// <summary>
	/// Создает информацию о создателе сервиса.
	/// </summary>
	/// <param name="creatorId">Идентификатор создателя сервиса.</param>
	/// <param name="type">Тип сервиса.</param>
	/// <param name="domain">Домен сервиса.</param>
	/// <returns>Результат создания информации о создателе сервиса.</returns>
	public static Result<ServiceCreatorInfo> Create(Guid creatorId, string type, string domain)
	{
		if (creatorId == Guid.Empty)
		{
			return Error.Validation("Идентификатор создателя сохраняемого элемента не может быть пустым.");
		}
		if (string.IsNullOrWhiteSpace(type))
		{
			return Error.Validation("Тип сохраняемого элемента не может быть пустым.");
		}
		if (string.IsNullOrWhiteSpace(domain))
		{
			return Error.Validation("Домен сохраняемого элемента не может быть пустым.");
		}
		if (type.Length > MAX_LENGTH)
		{
			return Error.Validation($"Тип сохраняемого элемента не может превышать {MAX_LENGTH} символов.");
		}
		return domain.Length > MAX_LENGTH
			? Error.Validation($"Домен сохраняемого элемента не может превышать {MAX_LENGTH} символов.")
			: new ServiceCreatorInfo(creatorId, type, domain);
	}
}
