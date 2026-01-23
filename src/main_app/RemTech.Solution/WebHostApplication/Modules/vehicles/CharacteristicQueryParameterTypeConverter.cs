using System.ComponentModel;
using System.Globalization;
using System.Text.Json;

namespace WebHostApplication.Modules.vehicles;

public sealed class CharacteristicQueryParameterTypeConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
		sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
	{
		if (value is string s)
		{
			using JsonDocument doc = JsonDocument.Parse(s);
			JsonElement property = doc.RootElement.GetProperty("id");
			if (property.ValueKind != JsonValueKind.String)
				throw new ArgumentException($"Invalid characteristics filter format: {s}");
			Guid id = property.GetGuid();

			property = doc.RootElement.GetProperty("value");
			if (property.ValueKind != JsonValueKind.String)
				throw new ArgumentException($"Invalid characteristics filter format: {s}");

			string ctxValue = property.GetString()!;
			return new CharacteristicQueryParameter(id, ctxValue);
		}

		throw new ArgumentException($"Invalid characteristics filter format: {value}");
	}
}
