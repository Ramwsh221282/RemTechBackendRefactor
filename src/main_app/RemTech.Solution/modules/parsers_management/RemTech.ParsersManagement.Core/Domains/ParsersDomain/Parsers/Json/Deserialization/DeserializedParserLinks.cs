using RemTech.Core.Shared.Primitives;
using RemTech.Json.Library.Deserialization;
using RemTech.Json.Library.Deserialization.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingStatistics;
using RemTech.ParsersManagement.Core.Common.ValueObjects.WorkingTimes;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkActivities;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Deserialization;

public sealed class DeserializedParserLinks
{
    private readonly ParserLinksBag _bag;

    public DeserializedParserLinks(IParser parser, DesJsonSource source)
    {
        using DesJsonArray array = new(source["links"]);
        IParserLink[] mappedLinks = array.MapEach<IParserLink>(element =>
        {
            ParserLinkIdentity identity = new DeserializedParserLinkIdentity(parser, element);
            ParserLinkStatistic statistic = new DeserializedParserLinkStatistics(element);
            ParserLinkUrl url = new DeserializedParserLinkUrl(element);
            ParserLinkActivity activity = new DeserializedParserLinkActivity(element);
            return new ParserLink(identity, url, statistic, activity);
        });
        _bag = new ParserLinksBag(mappedLinks);
    }

    public static implicit operator ParserLinksBag(DeserializedParserLinks links) => links._bag;
}

public sealed class DeserializedParserLinkIdentity
{
    private readonly ParserLinkIdentity _identity;

    public DeserializedParserLinkIdentity(IParser parser, DesJsonArrayElement element)
    {
        NotEmptyGuid id = NotEmptyGuid.New(new DesJsonGuid(element["id"]));
        Name linkName = new(NotEmptyString.New(new DesJsonString(element["name"])));
        _identity = new ParserLinkIdentity(parser, id, linkName);
    }

    public static implicit operator ParserLinkIdentity(DeserializedParserLinkIdentity identity) =>
        identity._identity;
}

public sealed class DeserializedParserLinkStatistics
{
    private readonly ParserLinkStatistic _statistic;

    public DeserializedParserLinkStatistics(DesJsonArrayElement element)
    {
        IncrementableNumber processed = new(
            PositiveInteger.New(new DesJsonInteger(element["processed"]))
        );
        PositiveLong total_seconds = PositiveLong.New(new DesJsonLong(element["total_seconds"]));
        Hour hours = new(PositiveInteger.New(new DesJsonInteger(element["hours"])));
        Minutes minutes = new(PositiveInteger.New(new DesJsonInteger(element["minutes"])));
        Seconds seconds = new(PositiveInteger.New(new DesJsonInteger(element["seconds"])));
        WorkingTime time = new(total_seconds, hours, minutes, seconds);
        _statistic = new ParserLinkStatistic(new WorkingStatistic(time, processed));
    }

    public static implicit operator ParserLinkStatistic(
        DeserializedParserLinkStatistics statistic
    ) => statistic._statistic;
}

public sealed class DeserializedParserLinkUrl
{
    private readonly ParserLinkUrl _url;

    public DeserializedParserLinkUrl(DesJsonArrayElement element)
    {
        _url = new ParserLinkUrl(new NotEmptyString(new DesJsonString(element["url"])));
    }

    public static implicit operator ParserLinkUrl(DeserializedParserLinkUrl url) => url._url;
}

public sealed class DeserializedParserLinkActivity
{
    private readonly ParserLinkActivity _activity;

    public DeserializedParserLinkActivity(DesJsonArrayElement element)
    {
        _activity = new ParserLinkActivity(new DesJsonBoolean(element["activity"]));
    }

    public static implicit operator ParserLinkActivity(DeserializedParserLinkActivity activity) =>
        activity._activity;
}
