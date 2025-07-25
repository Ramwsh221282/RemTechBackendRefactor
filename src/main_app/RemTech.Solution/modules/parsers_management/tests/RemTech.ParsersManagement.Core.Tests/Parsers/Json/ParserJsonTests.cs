using RemTech.Core.Shared.Primitives;
using RemTech.Json.Library.Deserialization;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Deserialization;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Json.Serialization;
using RemTech.ParsersManagement.Tests.Library;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;

namespace RemTech.ParsersManagement.Core.Tests.Parsers.Json;

public sealed class ParserJsonTests : IClassFixture<ParsersFixture>
{
    private readonly ParsersFixture _fixture;

    public ParserJsonTests(ParsersFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    private void Serealize_Deserialize_Parser_Success()
    {
        ParserTestingToolkit toolkit = new(_fixture.AccessLogger(), _fixture.Parsers());
        IParser parser = toolkit.CreateInitialParser();
        for (int index = 0; index < 3; index++)
        {
            ParserLinkUrl url = new(NotEmptyString.New($"Test Url {index}"));
            Name linkName = new(NotEmptyString.New($"Test Link {index}"));
            ParserLinkIdentity identity = new(parser, linkName);
            parser.Put(new ParserLink(identity, url));
        }
        string json = new ParserJson(parser);
        using DesJsonSource source = new(json);
        using DeserializedParser deserialized = new(source);
        IParser convParser = deserialized.Map();
        Guid origId = parser.Identification().ReadId();
        Guid resId = convParser.Identification().ReadId();
        Assert.Equal(origId, resId);
        string origName = parser.Identification().ReadName();
        string resName = convParser.Identification().ReadName();
        Assert.Equal(origName, resName);
        string origType = parser.Identification().ReadType().Read();
        string resType = convParser.Identification().ReadType().Read();
        Assert.Equal(origType, resType);
        string origState = parser.WorkState();
        string resState = convParser.WorkState();
        Assert.Equal(origState, resState);
        string origDomain = parser.Domain();
        string resDomain = convParser.Domain();
        Assert.Equal(origDomain, resDomain);
        int origProcessed = parser.WorkedStatistics().ProcessedAmount();
        int resProcessed = convParser.WorkedStatistics().ProcessedAmount();
        Assert.Equal(origProcessed, resProcessed);
        long origTotals = parser.WorkedStatistics().WorkedTime().Total();
        long resTotals = convParser.WorkedStatistics().WorkedTime().Total();
        Assert.Equal(origTotals, resTotals);
        int origHours = parser.WorkedStatistics().WorkedTime().Hours();
        int resHours = convParser.WorkedStatistics().WorkedTime().Hours();
        Assert.Equal(origHours, resHours);
        int origMinutes = parser.WorkedStatistics().WorkedTime().Minutes();
        int resMinutes = convParser.WorkedStatistics().WorkedTime().Minutes();
        Assert.Equal(origMinutes, resMinutes);
        int origSeconds = parser.WorkedStatistics().WorkedTime().Seconds();
        int resSeconds = convParser.WorkedStatistics().WorkedTime().Seconds();
        Assert.Equal(origSeconds, resSeconds);
        int origWaitDays = parser.WorkSchedule().WaitDays();
        int resWaitDays = convParser.WorkSchedule().WaitDays();
        Assert.Equal(origWaitDays, resWaitDays);
    }
}
