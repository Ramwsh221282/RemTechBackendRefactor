using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Spares.Domain.Models;

public sealed record SparePrice
{
    public long Value { get; }
    public bool IsNds { get; }

    private SparePrice(long value, bool isNds) => (Value, IsNds) = (value, isNds);

    public static Result<SparePrice> Create(long value, bool isNds) =>
        value <= 0
            ? Error.Validation("Цена запчасти должна быть больше 0.")
            : Result.Success(new SparePrice(value, isNds));
}
