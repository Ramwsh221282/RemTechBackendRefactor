using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Oems;

/// <summary>
/// OEM-номер запчасти.
/// </summary>
public sealed class SpareOem
{
	private SpareOem(string value, SpareOemId id)
	{
		Value = value;
		Id = id;
	}

	/// <summary>
	/// Значение OEM-номера.
	/// </summary>
	public string Value { get; }

	/// <summary>
	/// Идентификатор OEM-номера запчасти.
	/// </summary>
	public SpareOemId Id { get; }

	/// <summary>
	/// Создаёт OEM-номер из строки.
	/// </summary>
	/// <param name="value">Строковое значение OEM-номера.</param>
	/// <returns>Результат создания OEM-номера.</returns>
	public static Result<SpareOem> Create(string value)
	{
        return Create(value, Guid.NewGuid());
    }

	public static Result<SpareOem> Create(string value, Guid id)
	{
		Result<SpareOemId> spareOemIdResult = SpareOemId.FromGuid(id);
		if (spareOemIdResult.IsFailure)
		{
			return spareOemIdResult.Error;
		}

        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("OEM-номер запчасти не может быть пустым.");
        }
        
        return new SpareOem(value, spareOemIdResult.Value);
	}

    public SpareOem MakeWordCharactersUpper()
    {
        Span<char> upperCased = new Span<char>(new char[Value.Length]);
        Value.AsSpan().CopyTo(upperCased);
        
        for (int i = 0; i < upperCased.Length; i++)
        {
            char character = upperCased[i];
            if (char.IsLetter(character))
            {
                character = char.ToUpperInvariant(character);
                upperCased[i] = character;
            }
        }
        
        return new SpareOem(new string(upperCased), Id);
    }
}
