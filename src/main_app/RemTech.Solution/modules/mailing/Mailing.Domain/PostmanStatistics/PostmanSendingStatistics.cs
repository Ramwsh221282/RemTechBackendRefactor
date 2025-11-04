using RemTech.Core.Shared.Result;

namespace Mailing.Domain.PostmanStatistics;

public sealed record PostmanSendingStatistics(IDbPostmanData Data) : IPostmanSendingStatistics
{
    public bool LimitReached(out Error error)
    {
        error = Error.None();
        if (Data.CurrentSent == Data.Limit)
        {
            error = new Error(LimitReachedErrorMessage(), ErrorCodes.Conflict);
            return true;
        }

        return false;
    }

    private string LimitReachedErrorMessage() =>
        $"Сервис отправки по почты превысил ежедневный лимит отправок. Лимит: {Data.Limit}. Текущее: {Data.CurrentSent}";
}