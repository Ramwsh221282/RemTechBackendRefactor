using RemTech.Result.Pattern;
using Vehicles.Domain.CategoryContext.ValueObjects;

namespace Vehicles.Domain.CategoryContext.Errors;

public sealed record CategoryNameNotUniqueError(CategoryName Name)
    : Error($"Категория техники {Name} уже существует.", ErrorCodes.Conflict);
