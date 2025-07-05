namespace RemTech.ParsersManagement.DataSource.Adapter.DataAccessModels;

public sealed class ParserLinkDao
{
    public required Guid Id { get; set; }
    public required Guid ParserId { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required bool Activity { get; set; }
    public required int Processed { get; set; }
    public required long TotalSeconds { get; set; }
    public required int Hours { get; set; }
    public required int Minutes { get; set; }
    public required int Seconds { get; set; }

    // public IParserLink ToLink()
    // {
    //     ParserLinkId id = ParserLinkId.Concrete(Id);
    //     ParserId parserId = Core.Domains.ParsersDomain.Parsers.ParserIdentities.ParserId.Concrete(
    //         ParserId
    //     );
    //     Status<Name> name = Core.Common.ValueObjects.Name.Create(Name);
    //     ParserLinkIdentity identity = new(id, parserId, name);
    //     LinkActivity activity = LinkActivity.Match(Activity);
    //     LinkUrl url = LinkUrl.Create(Url);
    //     ParserLinkDetails details = new(activity, url);
    //     WorkingTime time = WorkingTime.New(TotalSeconds, Hours, Minutes, Seconds);
    //     WorkingStatistic statistic = WorkingStatistic.New(time, Processed);
    //     return new ParserLink(identity, details, statistic);
    // }
}
