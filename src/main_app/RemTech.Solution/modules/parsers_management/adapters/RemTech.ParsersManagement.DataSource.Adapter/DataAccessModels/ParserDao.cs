namespace RemTech.ParsersManagement.DataSource.Adapter.DataAccessModels;

public sealed class ParserDao
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string State { get; set; }
    public required int Processed { get; set; }
    public required long TotalSeconds { get; set; }
    public required int Hours { get; set; }
    public required int Minutes { get; set; }
    public required int Seconds { get; set; }
    public required DateTime LastRun { get; set; }
    public required DateTime NextRun { get; set; }
    public required int WaitDays { get; set; }
    public List<ParserLinkDao> Links { get; set; } = [];

    public void TryAddLink(ParserLinkDao link)
    {
        if (Links.Any(l => l.Id == link.Id))
            return;
        Links.Add(link);
    }

    // public IParser ToParser()
    // {
    //     ParserId id = ParserId.Concrete(Id);
    //     Name name = Core.Common.ValueObjects.Name.Create(Name);
    //     ParsingType type = ParsingType.Match(Type);
    //     ParserIdentityDetails identity = new(id, name, type);
    //     WorkingState state = WorkingState.New(State);
    //     WorkingSchedule schedule = WorkingSchedule.New(LastRun, NextRun, WaitDays);
    //     WorkingTime time = WorkingTime.New(TotalSeconds, Hours, Minutes, Seconds);
    //     WorkingStatistic statistic = WorkingStatistic.New(time, Processed);
    //     ParserWorkDetails workDetails = new ParserWorkDetails(state, schedule, statistic);
    //     IEnumerable<IParserLink> links = Links.Select(l => l.ToLink());
    //     return new Parser(identity, workDetails, links);
    // }
}
