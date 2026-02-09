using System.ComponentModel;

namespace WebHostApplication.Modules.Vehicles;

/// <summary>
/// Параметр запроса характеристики транспортного средства.
/// </summary>
/// <param name="Id">Идентификатор характеристики</param>
/// <param name="Value">Значение характеристики</param>
[TypeConverter(typeof(CharacteristicQueryParameterTypeConverter))]
public record CharacteristicQueryParameter(Guid Id, string Value);
