using ParsersControl.Core.ParserStatisticsManagement.Contracts;
using ParsersControl.Core.ParserStatisticsManagement.Defaults;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserStatisticsManagement;

public sealed class ParserStatistic(ParserStatisticData data)
{
    private readonly ParserStatisticData _data = data;
    
    private readonly IParserStatisticsCreatedEventListener _onCreated =
        new NoneParserStatisticsCreatedEventListener();
    
    private readonly IParserStatisticsUpdatedEventListener _onUpdated =
        new NoneParserStatisticsUpdatedEventListener();

    public async Task<Result<ParserStatistic>> Register(CancellationToken ct = default)
    {
        Result<Unit> validation = Validate();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> registering = await _onCreated.React(_data, ct);
        return registering.IsFailure ? registering.Error : this;
    }

    public async Task<Result<ParserStatistic>> UpdateProcessedItems(int processed, CancellationToken ct = default)
    {
        ParserStatistic updated = new ParserStatistic(_data with { Processed = processed });
        Result<Unit> validation = updated.Validate();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> updating = await _onUpdated.React(updated._data, ct);
        return updating.IsFailure ? updating.Error : updated;
    }

    public async Task<Result<ParserStatistic>> UpdateElapsedSeconds(long seconds, CancellationToken ct = default)
    {
        ParserStatistic updated = new ParserStatistic(_data with { TotalSecondsElapsed = seconds });
        Result<Unit> validation = updated.Validate();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> updating = await _onUpdated.React(updated._data, ct);
        return updating.IsFailure ? updating.Error : updated;
    }

    public async Task<Result<ParserStatistic>> Reset(CancellationToken ct = default)
    {
        ParserStatistic updated = new ParserStatistic(_data with { Processed = 0, TotalSecondsElapsed = 0 });
        Result<Unit> validation = updated.Validate();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> updating = await _onUpdated.React(updated._data, ct);
        return updating.IsFailure ? updating.Error : updated;
    }

    public void Write(
        Action<Guid>? writeId = null, 
        Action<int>? writeProcessed = null, 
        Action<long>? writeElapsedSeconds = null)
    {
        writeId?.Invoke(_data.ParserId);
        writeProcessed?.Invoke(_data.Processed);
        writeElapsedSeconds?.Invoke(_data.TotalSecondsElapsed);
    }

    public ParserStatistic AddListener(IParserStatisticsUpdatedEventListener onUpdated)
    {
        return new ParserStatistic(this, onUpdated);
    }

    public ParserStatistic AddListener(IParserStatisticsCreatedEventListener onCreated)
    {
        return new ParserStatistic(this, onCreated);
    }
    
    public ParserStatistic(ParserStatistic statistic, IParserStatisticsCreatedEventListener onCreated)
        : this(statistic._data)
    {
        _onCreated = onCreated;
    }

    public ParserStatistic(ParserStatistic statistic, IParserStatisticsUpdatedEventListener onUpdated)
        : this(statistic._data)
    {
        _onUpdated = onUpdated;
    }

    public bool ProcessedEqualsTo(int processed)
    {
        return _data.Processed == processed;
    }

    public bool ElapsedEqualsTo(long elapsedSeconds)
    {
        return _data.TotalSecondsElapsed == elapsedSeconds;
    }
    
    private Result<Unit> Validate()
    {
        const string processedName = "Число обработанных страниц";
        const string elapsedTimeName = "Затраченное время в секундах";
        const string idName = "Идентификатор парсера";
        List<string> errors = [];
        if (_data.Processed < 0) errors.Add(Error.InvalidFormat(processedName));
        if (_data.TotalSecondsElapsed < 0) errors.Add(Error.InvalidFormat(elapsedTimeName));
        if (_data.ParserId == Guid.Empty) errors.Add(Error.NotSet(idName));
        return errors.Count == 0 ? Unit.Value : Error.Validation(errors);
    }
}