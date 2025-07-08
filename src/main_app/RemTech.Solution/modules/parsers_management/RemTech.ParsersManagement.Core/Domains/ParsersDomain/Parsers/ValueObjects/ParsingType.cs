using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;

public sealed class ParsingType
{
    private static readonly ParsingType[] _allowedTypes = [Spares(), Transport()];

    private readonly NotEmptyString _type;

    private ParsingType(NotEmptyString type) => _type = type;

    public NotEmptyString Read() => _type;

    public static ParsingType Spares() => new(NotEmptyString.New("Запчасти"));

    public static ParsingType Transport() => new(NotEmptyString.New("Техника"));

    public static Status<ParsingType> New(NotEmptyString input)
    {
        ParsingType? type = _allowedTypes.FirstOrDefault(t => t._type.Same(input));
        return type == null
            ? new Error("Неподдерживаемый тип парсинга.", ErrorCodes.Validation)
            : type;
    }

    public override bool Equals(object? obj) =>
        obj switch
        {
            null => false,
            ParsingType pt => pt._type.Equals(_type),
            _ => false,
        };

    public override int GetHashCode() => _type.GetHashCode();
}
