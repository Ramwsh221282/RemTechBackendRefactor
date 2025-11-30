using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using ParsersControl.Core.ParserWorkStateManagement.Defaults;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkStateManagement;

public sealed class ParserWorkTurner(ParserWorkTurnerState state)
{
    private readonly ParserWorkTurnerState _state = state;
    private readonly IParserStateChangedEventListener _stateChanged = 
        new NoneParserStateChangedEventListener();
    
    public async Task<Result<ParserWorkTurner>> Enable(CancellationToken ct = default)
    {
        return await UpdateState(s => 
                s.SwitchState(new ParserState.Enabled()), 
            (res, t) =>
                _stateChanged.React(res, t), ct);
    }

    public async Task<Result<ParserWorkTurner>> Disable(CancellationToken ct = default)
    {
        return await UpdateState(s => 
                s.SwitchState(new ParserState.Disabled()), 
            (res, t) =>
                _stateChanged.React(res, t), ct);
    }

    public async Task<Result<ParserWorkTurner>> StartWait(CancellationToken ct = default)
    {
        return await UpdateState(
            s => s.SwitchState(new ParserState.Waiting()), 
            (res, t) => _stateChanged.React(res, t), ct
            );
    }

    public async Task<Result<ParserWorkTurner>> StartWork(CancellationToken ct = default)
    {
        return await UpdateState(s => 
                s.SwitchState(new ParserState.Working()), 
            (res, t) => 
                _stateChanged.React(res, t), ct);
    }
    
    public async Task<Result<ParserWorkTurner>> StopWork(CancellationToken ct = default)
    {
        return await UpdateState(s => 
                s.SwitchState(new ParserState.Disabled()), 
            (res, t) =>
                _stateChanged.React(res, t), ct);
    }

    public async Task<Result<ParserWorkTurner>> PermanentStopWork(CancellationToken ct)
    {
        ParserWorkTurnerState updated = _state with { State = new ParserState.Disabled() };
        ParserWorkTurner parserWorkTurner = new(updated);
        Result<Unit> react = await _stateChanged.React(updated, ct);
        return react.IsFailure ? react.Error : parserWorkTurner;
    }

    public void Write(
        Action<Guid>? writeId = null,
        Action<string>? writeState = null
        )
    {
        writeId?.Invoke(_state.Id);
        writeState?.Invoke(_state.State.Value);
    }
    
    private async Task<Result<ParserWorkTurner>> UpdateState(
        Func<ParserWorkTurnerState, Result<ParserWorkTurnerState>> stateSwitchFn,
        Func<ParserWorkTurnerState, CancellationToken, Task<Result<Unit>>> listenerReactFn,
        CancellationToken ct
        )
    {
        Result<ParserWorkTurnerState> dataRes = stateSwitchFn(_state);
        if (dataRes.IsFailure) return dataRes.Error;
        Result<Unit> react = await listenerReactFn(dataRes.Value, ct);
        if (react.IsFailure) return react.Error;
        return new ParserWorkTurner(dataRes);
    }

    public ParserWorkTurner AddListener(IParserStateChangedEventListener? stateChanged = null) => new(this, stateChanged);
    
    private ParserWorkTurner(
        ParserWorkTurner origin,
        IParserStateChangedEventListener? stateChanged = null
        )
        : this(origin._state)
    {
        _stateChanged = stateChanged ?? origin._stateChanged;
    }
}