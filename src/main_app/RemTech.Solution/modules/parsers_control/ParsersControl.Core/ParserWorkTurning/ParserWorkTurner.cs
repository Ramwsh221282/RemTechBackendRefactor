using ParsersControl.Core.ParserWorkTurning.Contracts;
using ParsersControl.Core.ParserWorkTurning.Defaults;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkTurning;

public sealed class ParserWorkTurner(ParserWorkTurnerState state)
{
    private readonly IOnParserEnabledEventListener _onEnabled =
            new NoneOnParserEnabledEventListener();
    
    private readonly IOnParserDisabledEventListener _onDisabled =
            new NoneOnParserDisabledEventListener();
    
    private readonly IOnParserStartWaitingEventListener _onStartWait =
            new NoneOnParsedStartWaitingEventListener();
    
    private readonly IOnParserStartWorkEventListener _onStartWork =
            new NoneOnParserStartWorkEventListener();
    
    private readonly IOnParserStopWorkEventListener _onStopWork =
            new NoneOnParserStopWorkEventListener();
    
    private readonly IOnPermanentStopWorkEventListener _onPermanentStopWork =
            new NoneOnPermanentStopWorkEventListener();
    
    public async Task<Result<ParserWorkTurner>> Enable(CancellationToken ct = default)
    {
        return await UpdateState(s => 
                s.SwitchState(new ParserState.Enabled()), 
            (res, t) =>
                _onEnabled.React(res, t), ct);
    }

    public async Task<Result<ParserWorkTurner>> Disable(CancellationToken ct = default)
    {
        return await UpdateState(s => 
                s.SwitchState(new ParserState.Disabled()), 
            (res, t) =>
                _onDisabled.React(res, t), ct);
    }

    public async Task<Result<ParserWorkTurner>> StartWait(CancellationToken ct = default)
    {
        return await UpdateState(s => 
                s.SwitchState(new ParserState.Waiting()), 
            (res, t) =>
                _onStartWait.React(res, t), ct);
    }

    public async Task<Result<ParserWorkTurner>> StartWork(CancellationToken ct = default)
    {
        return await UpdateState(s => 
                s.SwitchState(new ParserState.Working()), 
            (res, t) => 
                _onStartWork.React(res, t), ct);
    }
    
    public async Task<Result<ParserWorkTurner>> StopWork(CancellationToken ct = default)
    {
        return await UpdateState(s => 
                s.SwitchState(new ParserState.Disabled()), 
            (res, t) =>
                _onStopWork.React(res, t), ct);
    }

    public async Task<Result<ParserWorkTurner>> PermanentStopWork(CancellationToken ct)
    {
        ParserWorkTurnerState @updated = state with { State = new ParserState.Disabled() };
        ParserWorkTurner parserWorkTurner = new(@updated);
        Result<Unit> react = await _onPermanentStopWork.React(@updated, ct);
        return react.IsFailure ? react.Error : parserWorkTurner;
    }

    public void Write(
        Action<Guid>? writeId = null,
        Action<string>? writeState = null
        )
    {
        writeId?.Invoke(state.Id);
        writeState?.Invoke(state.State.Value);
    }
    
    private async Task<Result<ParserWorkTurner>> UpdateState(
        Func<ParserWorkTurnerState, Result<ParserWorkTurnerState>> stateSwitchFn,
        Func<Result<ParserWorkTurnerState>, CancellationToken, Task<Result<Unit>>> listenerReactFn,
        CancellationToken ct
        )
    {
        Result<ParserWorkTurnerState> dataRes = stateSwitchFn(state);
        if (dataRes.IsFailure) return dataRes.Error;
        Result<Unit> react = await listenerReactFn(dataRes, ct);
        if (react.IsFailure) return react.Error;
        return new ParserWorkTurner(dataRes);
    }
}