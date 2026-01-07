using System.ComponentModel;

namespace WebHostApplication.Modules.vehicles;

[TypeConverter(typeof(CharacteristicQueryParameterTypeConverter))]
public record CharacteristicQueryParameter(Guid Id, string Value);