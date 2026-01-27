using System.ComponentModel;

namespace WebHostApplication.Modules.vehicles;

/// <summary>
/// Параметр запроса характеристики транспортного средства.
/// </summary>
[TypeConverter(typeof(CharacteristicQueryParameterTypeConverter))]
public record CharacteristicQueryParameter(Guid Id, string Value);
