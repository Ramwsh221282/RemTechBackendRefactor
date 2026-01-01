using System.ComponentModel;

namespace Vehicles.WebApi.Controllers;

[TypeConverter(typeof(CharacteristicQueryParameterTypeConverter))]
public record CharacteristicQueryParameter(Guid Id, string Value);