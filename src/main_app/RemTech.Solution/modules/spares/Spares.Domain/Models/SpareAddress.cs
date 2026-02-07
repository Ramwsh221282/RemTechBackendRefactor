using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

/// <summary>
/// Адрес запчасти.
/// </summary>
public sealed record SpareAddress
{
	private SpareAddress(string value)
	{
		Value = value;
	}

	/// <summary>
	/// Значение адреса.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Создаёт адрес запчасти.
	/// </summary>
	/// <param name="value">Значение адреса.</param>
	/// <returns>Результат создания адреса запчасти.</returns>
	public static Result<SpareAddress> Create(string value)
	{
		return string.IsNullOrWhiteSpace(value)
			? Error.Validation("Адрес не может быть пустым")
			: new SpareAddress(value);
	}
}

public sealed record SpareAddressId
{
    public Guid Value { get; }
    private SpareAddressId(Guid id)
    {
        Value = id;
    }

    public static Result<SpareAddressId> Create(Guid id)
    {
        if (id == Guid.Empty)
        {
            return Error.Validation("Идентификатор адреса не может быть пустым");
        }
    
        return new SpareAddressId(id);
    }
}
