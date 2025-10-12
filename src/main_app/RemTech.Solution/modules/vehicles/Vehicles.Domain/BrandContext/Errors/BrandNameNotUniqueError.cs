using RemTech.Result.Pattern;
using Vehicles.Domain.BrandContext.ValueObjects;

namespace Vehicles.Domain.BrandContext.Errors;

public sealed record BrandNameNotUniqueError(BrandName Name)
    : Error($"Бренд с названием: {Name.Name} уже существует.", ErrorCodes.Conflict);
