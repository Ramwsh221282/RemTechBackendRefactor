using System.ComponentModel;
using System.Globalization;

namespace WebHostApplication.Modules.Telemetry;

public sealed class SortDictionaryQueryParameterConverter : TypeConverter
{
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
	{
		return sourceType == typeof(string);
	}

	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
	{
		if (value is string stringValue)
		{
			return SortDictionary.FromString(stringValue);
		}

		return base.ConvertFrom(context, culture, value);
	}
}
