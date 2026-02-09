using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

namespace WebHostApplication.Modules.Vehicles;

/// <summary>
/// Конвертер для преобразования строки в параметр характеристики транспортного средства.
/// </summary>
public sealed class CharacteristicQueryParameterTypeConverter : TypeConverter
{
	/// <inheritdoc/>
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
	{
		return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
	}

	/// <inheritdoc/>
	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
	{
		if (value is string s)
		{
			using JsonDocument doc = JsonDocument.Parse(s);
			JsonElement property = doc.RootElement.GetProperty("id");
			if (property.ValueKind != JsonValueKind.String)
			{
				throw new ArgumentException($"Invalid characteristics filter format: {s}");
			}

			Guid id = property.GetGuid();
			property = doc.RootElement.GetProperty("value");
			if (property.ValueKind != JsonValueKind.String)
			{
				throw new ArgumentException($"Invalid characteristics filter format: {s}");
			}

			string ctxValue = property.GetString()!;
			return new CharacteristicQueryParameter(id, ctxValue);
		}

		throw new ArgumentException($"Invalid characteristics filter format: {value}");
	}
}
