using RemTech.Result.Pattern;

namespace Vehicles.Domain.LocationContext.Errors;

public sealed record LocationAddressPartsAreNotUniqueError(IEnumerable<string> RepeatableValues)
    : Error(
        $"Адрес должен состоять из не повторяющихся частей. Не уникальные значения: {string.Join(", ", RepeatableValues)}",
        ErrorCodes.Validation
    );
