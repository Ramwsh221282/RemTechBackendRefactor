using System.Text.Json;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Cache.Adapter.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        Name name = new(NotEmptyString.New("Test parser"));
        ParsingType type = ParsingType.New(NotEmptyString.New("Техника"));
        ParserServiceDomain domain = new ParserServiceDomain(NotEmptyString.New("Test"));
        IParser parser = new Parser(name, type, domain);
        parser.Put(
            new ParserLink(
                new ParserLinkIdentity(parser, new Name(NotEmptyString.New("Test link"))),
                new ParserLinkUrl(NotEmptyString.New("Test Url"))
            )
        );
        JsonSourceParser jsonSourceParser = new(parser);
        Json json = jsonSourceParser.Serialize();
        JsonSourceParser deserialized = new(json);
        int bpoint = 0;
    }
}

public sealed class Json
{
    private readonly string _json;

    public Json(string json) => _json = json;

    public string Print() => _json;
}

public interface IJsonSource
{
    Json Serialize();
}

public sealed class JsonSourceMakingParser : IParser, IJsonSource
{
    private readonly IParser _parser;
    private JsonSourceParser _jsonSource;

    public JsonSourceMakingParser(IParser parser, JsonSourceParser jsonSource)
    {
        _parser = parser;
        _jsonSource = jsonSource;
    }

    public ParserIdentity Identification() => _parser.Identification();

    public ParserStatistic WorkedStatistics() => _parser.WorkedStatistics();

    public ParserSchedule WorkSchedule() => _parser.WorkSchedule();

    public ParserState WorkState() => _parser.WorkState();

    public ParserLinksBag OwnedLinks() => _parser.OwnedLinks();

    public ParserServiceDomain Domain() => _parser.Domain();

    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IParserLink link) =>
        _parser.IncreaseProcessed(link);

    public Status ChangeState(NotEmptyString stateString) => _parser.ChangeState(stateString);

    public Status Enable() => _parser.Enable();

    public Status Disable() => _parser.Disable();

    public Status ChangeWaitDays(PositiveInteger waitDays) => _parser.ChangeWaitDays(waitDays);

    public Status<IParserLink> Put(IParserLink link) => _parser.Put(link);

    public Status<IParserLink> Drop(IParserLink link) => _parser.Drop(link);

    public Status<IParserLink> ChangeActivityOf(IParserLink link, bool nextActivity) =>
        _parser.ChangeActivityOf(link, nextActivity);

    public Status<IParserLink> Finish(IParserLink link, PositiveLong elapsed) =>
        _parser.Finish(link, elapsed);

    public Status Stop() => _parser.Stop();

    public Status Start() => _parser.Start();

    public Json Serialize() => _jsonSource.Serialize();
}

public sealed class JsonSourceParser : IJsonSource
{
    public Guid id { get; set; }
    public string name { get; set; }
    public string type { get; set; }
    public string state { get; set; }
    public string domain { get; set; }
    public int processed { get; set; }
    public long totalSeconds { get; set; }
    public int hours { get; set; }
    public int minutes { get; set; }
    public int seconds { get; set; }
    public int waitDays { get; set; }
    public DateTime nextRun { get; set; }
    public DateTime lastRun { get; set; }
    public List<JsonParserLink> links { get; set; }

    public JsonSourceParser(IParser parser)
    {
        id = parser.Identification().ReadId();
        name = parser.Identification().ReadName().NameString();
        type = parser.Identification().ReadType().Read();
        state = parser.WorkState();
        domain = parser.Domain();
        processed = parser.WorkedStatistics().ProcessedAmount().Read();
        totalSeconds = parser.WorkedStatistics().ProcessedAmount().Read();
        hours = parser.WorkedStatistics().ProcessedAmount().Read();
        minutes = parser.WorkedStatistics().ProcessedAmount().Read();
        seconds = parser.WorkedStatistics().ProcessedAmount().Read();
        waitDays = parser.WorkedStatistics().ProcessedAmount().Read();
        nextRun = parser.WorkSchedule().NextRun();
        lastRun = parser.WorkSchedule().LastRun();
        links = parser.OwnedLinks().Read().Select(l => new JsonParserLink(l)).ToList();
    }

    public JsonSourceParser(Json json)
    {
        string jsonString = json.Print();
        using JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        id = jsonDocument.RootElement.GetProperty(nameof(id)).GetGuid();
        name = jsonDocument.RootElement.GetProperty(nameof(name)).GetString()!;
        type = jsonDocument.RootElement.GetProperty(nameof(type)).GetString()!;
        state = jsonDocument.RootElement.GetProperty(nameof(state)).GetString()!;
        domain = jsonDocument.RootElement.GetProperty(nameof(domain)).GetString()!;
        processed = jsonDocument.RootElement.GetProperty(nameof(processed)).GetInt32();
        totalSeconds = jsonDocument.RootElement.GetProperty(nameof(totalSeconds)).GetInt64();
        hours = jsonDocument.RootElement.GetProperty(nameof(hours)).GetInt32();
        minutes = jsonDocument.RootElement.GetProperty(nameof(minutes)).GetInt32();
        seconds = jsonDocument.RootElement.GetProperty(nameof(seconds)).GetInt32();
        waitDays = jsonDocument.RootElement.GetProperty(nameof(waitDays)).GetInt32();
        nextRun = jsonDocument.RootElement.GetProperty(nameof(nextRun)).GetDateTime();
        lastRun = jsonDocument.RootElement.GetProperty(nameof(lastRun)).GetDateTime();
        links = new List<JsonParserLink>(
            jsonDocument.RootElement.GetProperty(nameof(links)).GetArrayLength()
        );
        foreach (
            var jsonLink in jsonDocument.RootElement.GetProperty(nameof(links)).EnumerateArray()
        )
            links.Add(new JsonParserLink(jsonLink));
    }

    public Json Serialize() => new(JsonSerializer.Serialize(this));
}

public sealed class JsonParserLink
{
    public Guid id { get; set; }
    public Guid parserId { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public bool activity { get; set; }
    public int processed { get; set; }
    public long totalSeconds { get; set; }
    public int hours { get; set; }
    public int minutes { get; set; }
    public int seconds { get; set; }

    public JsonParserLink(IParserLink link)
    {
        id = link.Identification().ReadId();
        parserId = link.Identification().OwnerIdentification().ReadId();
        name = link.Identification().ReadName();
        url = link.ReadUrl().Read();
        activity = link.Activity();
        processed = link.WorkedStatistic().ProcessedAmount();
        totalSeconds = link.WorkedStatistic().WorkedTime().Total();
        hours = link.WorkedStatistic().WorkedTime().Hours();
        minutes = link.WorkedStatistic().WorkedTime().Minutes();
        seconds = link.WorkedStatistic().WorkedTime().Seconds();
    }

    public JsonParserLink(JsonElement jsonLink)
    {
        id = jsonLink.GetProperty(nameof(id)).GetGuid();
        parserId = jsonLink.GetProperty(nameof(parserId)).GetGuid();
        name = jsonLink.GetProperty(nameof(name)).GetString()!;
        url = jsonLink.GetProperty(nameof(url)).GetString()!;
        activity = jsonLink.GetProperty(nameof(activity)).GetBoolean();
        processed = jsonLink.GetProperty(nameof(processed)).GetInt32();
        totalSeconds = jsonLink.GetProperty(nameof(totalSeconds)).GetInt64();
        hours = jsonLink.GetProperty(nameof(hours)).GetInt32();
        minutes = jsonLink.GetProperty(nameof(minutes)).GetInt32();
        seconds = jsonLink.GetProperty(nameof(seconds)).GetInt32();
    }
}
