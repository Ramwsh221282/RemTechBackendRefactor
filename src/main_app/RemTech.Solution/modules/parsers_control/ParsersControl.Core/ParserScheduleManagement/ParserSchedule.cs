using ParsersControl.Core.ParserScheduleManagement.Contracts;
using ParsersControl.Core.ParserScheduleManagement.Defaults;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserScheduleManagement;

public sealed class ParserSchedule(ParserScheduleData data)
{
    private readonly ParserScheduleData _data = data;
    
    private readonly IParserScheduleUpdatedEventListener _onUpdated =
        new NoneParserScheduleUpdatedListener();
    
    private readonly IParserScheduleCreatedEventListener _onCreated =
        new NoneParserScheduleCreatedEventListener();

    public async Task<Result<ParserSchedule>> Register(CancellationToken ct = default)
    {
        Result<Unit> validation = Validate();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> processing = await _onCreated.React(_data, ct);
        return processing.IsFailure ? processing.Error : this;
    }

    public async Task<Result<ParserSchedule>> Update(
        DateTime? end = null,
        int? waitDays = null,
        CancellationToken ct = default)
    {
        ParserScheduleData updatingData = _data;
        if (end.HasValue) updatingData = updatingData with { End = end.Value };
        if (waitDays.HasValue) updatingData = updatingData with { WaitDays = waitDays.Value };
        ParserSchedule schedule = new(updatingData);
        Result<Unit> validation = schedule.Validate();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> processing = await _onUpdated.React(schedule._data, ct);
        return processing.IsFailure ? processing.Error : schedule;
    }

    public void Write(
        Action<Guid>? writeId = null, 
        Action<DateTime?>? writeEnd = null,
        Action<int?>? writeDays = null)
    {
        writeId?.Invoke(_data.Id);
        writeEnd?.Invoke(_data.End);
        writeDays?.Invoke(_data.WaitDays);
    }

    public ParserSchedule AddListener(IParserScheduleCreatedEventListener listener)
    {
        return new ParserSchedule(this, listener);
    }

    public ParserSchedule AddListener(IParserScheduleUpdatedEventListener listener)
    {
        return new ParserSchedule(this, listener);
    }
    
    private ParserSchedule(ParserSchedule schedule, IParserScheduleCreatedEventListener listener)
    : this(schedule._data)
    {
        _onUpdated = schedule._onUpdated;
        _onCreated = listener;
    }

    private ParserSchedule(ParserSchedule schedule, IParserScheduleUpdatedEventListener listener)
    : this(schedule._data)
    {
        _onCreated = schedule._onCreated;
        _onUpdated = listener;
    }
    
    private Result<Unit> Validate()
    {
        const string idName = "идентификатор парсера";
        const string endName = "дата окончания";
        const string daysName = "дни ожидания";
        List<string> errors = [];
        if (_data.Id == Guid.Empty) errors.Add(Error.NotSet(idName));
        if (_data.End.HasValue)
            if (_data.End.Value == DateTime.MinValue || _data.End.Value == DateTime.MaxValue)
                errors.Add(Error.InvalidFormat(endName));
        if (_data.WaitDays < 0 || _data.WaitDays > 7) errors.Add(Error.InvalidFormat(daysName));
        return errors.Count == 0 ? Unit.Value : Error.Validation(errors);
    }
}