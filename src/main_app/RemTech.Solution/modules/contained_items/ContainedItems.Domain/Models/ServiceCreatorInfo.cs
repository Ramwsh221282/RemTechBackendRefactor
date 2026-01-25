using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ContainedItems.Domain.Models;

public sealed record ServiceCreatorInfo
{
	private const int MaxLength = 128;

	private ServiceCreatorInfo(Guid creatorId, string type, string domain)
	{
		CreatorId = creatorId;
		Type = type;
		Domain = domain;
	}

	public Guid CreatorId { get; }
	public string Type { get; }
	public string Domain { get; }

	public static Result<ServiceCreatorInfo> Create(Guid creatorId, string type, string domain)
	{
		if (creatorId == Guid.Empty)
			return Error.Validation("Идентификатор создателя сохраняемого элемента не может быть пустым.");
		if (string.IsNullOrWhiteSpace(type))
			return Error.Validation("Тип сохраняемого элемента не может быть пустым.");
		if (string.IsNullOrWhiteSpace(domain))
			return Error.Validation("Домен сохраняемого элемента не может быть пустым.");
		if (type.Length > MaxLength)
			return Error.Validation($"Тип сохраняемого элемента не может превышать {MaxLength} символов.");
		return domain.Length > MaxLength
			? Error.Validation($"Домен сохраняемого элемента не может превышать {MaxLength} символов.")
			: new ServiceCreatorInfo(creatorId, type, domain);
	}
}
