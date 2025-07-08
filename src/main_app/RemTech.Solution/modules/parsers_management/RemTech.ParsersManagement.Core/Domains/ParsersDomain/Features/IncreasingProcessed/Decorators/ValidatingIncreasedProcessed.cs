using RemTech.Core.Shared.Functional;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Decorators;

public sealed class ValidatingIncreasedProcessed(IIncreaseProcessed inner) : IIncreaseProcessed
{
    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IncreaseProcessed increase) =>
        new ValidatingOperation(increase).Process(() => inner.IncreaseProcessed(increase));
}
