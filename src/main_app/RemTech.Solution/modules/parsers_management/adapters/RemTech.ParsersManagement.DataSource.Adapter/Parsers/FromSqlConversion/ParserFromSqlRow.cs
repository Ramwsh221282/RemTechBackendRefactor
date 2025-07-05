using System.Data.Common;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.DataSource.Adapter.Parsers.FromSqlConversion;

public sealed class ParserFromSqlRow(DbDataReader reader)
{
    private MaybeBag<IParser> _parser = new();
    private readonly List<IParserLink> _links = [];
    private readonly HasParserLinkFromSqlRow _hasLink = new(reader);
    private readonly ParserLinkFromSqlRow _linkFromSqlRow = new(reader);
    private readonly ParserIdentityFromSqlRow _identityFromSqlRow = new(reader);
    private readonly ParserStatisticFromSqlRow _statisticFromSqlRow = new(reader);
    private readonly ParserScheduleFromSqlRow _parserSchedule = new(reader);
    private readonly ParserStateFromSqlRow _parserStateFromSqlRow = new(reader);

    public async Task<IParser> Read()
    {
        ReadParser();
        await ReadParserLink();
        return _parser.Take();
    }

    private async Task ReadParserLink()
    {
        if (!_parser.Any())
            return;
        if (!await _hasLink.Read())
            return;
        _parser.Take().Put(_linkFromSqlRow.Read());
    }

    public void ReadParser()
    {
        if (_parser.Any())
            return;
        IParser parser = new Parser(
            _identityFromSqlRow.Read(),
            _statisticFromSqlRow.Read(),
            _parserSchedule.Read(),
            _parserStateFromSqlRow.Read()
        );
        _parser = _parser.Put(parser);
    }
}
