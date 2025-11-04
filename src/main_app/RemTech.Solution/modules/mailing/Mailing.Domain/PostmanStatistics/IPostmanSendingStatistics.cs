using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostmanStatistics;

public interface IPostmanSendingStatistics
{
    IDbPostmanData Data { get; }
    bool LimitReached(out Error error);
}