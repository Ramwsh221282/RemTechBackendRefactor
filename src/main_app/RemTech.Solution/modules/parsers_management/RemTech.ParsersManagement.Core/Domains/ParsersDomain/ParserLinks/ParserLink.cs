using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.Primitives.Comparing;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingStatistics;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;

public sealed class ParserLink : IParserLink
{
    private readonly ParserLinkIdentity _identity;
    private readonly ParserLinkUrl _url;
    private ParserLinkStatistic _statistic;
    private ParserLinkActivity _activity;

    public ParserLink(ParserLinkIdentity identity, ParserLinkUrl url)
    {
        _identity = identity;
        _url = url;
        _activity = new ParserLinkActivity(false);
        _statistic = new ParserLinkStatistic(new WorkingStatistic());
    }

    public ParserLink(
        ParserLinkIdentity identity,
        ParserLinkUrl url,
        ParserLinkStatistic statistic,
        ParserLinkActivity activity
    )
    {
        _identity = identity;
        _url = url;
        _statistic = statistic;
        _activity = activity;
    }

    public ParserLinkStatistic WorkedStatistic() => _statistic;

    public ParserLinkIdentity Identification() => _identity;

    public ParserLinkActivity Activity() => _activity;

    public ParserLinkUrl ReadUrl() => _url;

    public Status OtherActivity(bool other)
    {
        if (other == _activity)
            return Error.Conflict("У ссылки такая же активность, что и была.");
        _activity = new ParserLinkActivity(other);
        return Status.Success();
    }

    public Status Finished(PositiveLong elapsed)
    {
        if (!_activity)
            return Error.Conflict("Ссылка неактивна, нельзя закончить ей работу.");
        _statistic += elapsed;
        return Status.Success();
    }

    public bool SameBy(ICompare compare) => compare.Equality();
}
