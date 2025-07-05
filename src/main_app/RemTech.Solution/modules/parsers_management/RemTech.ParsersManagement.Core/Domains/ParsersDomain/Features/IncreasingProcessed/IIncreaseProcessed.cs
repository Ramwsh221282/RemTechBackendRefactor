using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed;

public interface IIncreaseProcessed
{
    Status<ParserStatisticsIncreasement> IncreaseProcessed(IncreaseProcessed increase);
}