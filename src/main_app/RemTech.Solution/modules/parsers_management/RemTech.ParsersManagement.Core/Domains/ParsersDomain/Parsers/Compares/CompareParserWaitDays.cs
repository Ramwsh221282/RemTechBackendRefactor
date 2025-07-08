using RemTech.Core.Shared.Primitives.Comparing;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Compares;

public sealed class CompareParserWaitDays : ICompare
{
    private readonly bool _compare;

    public CompareParserWaitDays(IParser parser, int related)
        : this(parser.WorkSchedule().WaitDays(), related) { }

    public CompareParserWaitDays(int current, int related)
    {
        _compare = current == related;
    }

    public bool Equality() => _compare;

    public static implicit operator bool(CompareParserWaitDays compare) => compare._compare;
}
