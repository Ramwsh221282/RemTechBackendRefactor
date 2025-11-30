using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserStateManagement.Contracts;
using ParsersControl.Core.ParserStateManagement.Defaults;
using ParsersControl.Core.ParserWorkStateManagement;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserStateManagement;

public sealed class StatefulParser(RegisteredParser registered, ParserWorkTurner workTurner)
{
    private readonly RegisteredParser _registered = registered;
    private readonly ParserWorkTurner _workTurner = workTurner;

    private readonly IOnStatefulParserStateChangedEventListener _listener =
        new NoneOnStatefulParserStateChangedEventListener();

    public async Task<Result<Unit>> Enable(CancellationToken ct = default) =>
        await UpdateState(t => t.Enable(ct), ct);

    public async Task<Result<Unit>> Disable(CancellationToken ct = default) =>
        await UpdateState(t => t.Disable(ct), ct);

    public async Task<Result<Unit>> StartWaiting(CancellationToken ct = default) =>
        await UpdateState(t => t.StartWait(ct), ct);

    public async Task<Result<Unit>> PermanentDisable(CancellationToken ct = default) =>
        await UpdateState(t => t.PermanentStopWork(ct), ct);

    public async Task<Result<Unit>> StartWork(CancellationToken ct = default) =>
        await UpdateState(t => t.StartWork(ct), ct);

    public static async Task<Result<StatefulParser>> FromStorage(
        Func<IStatefulParsersStorage, CancellationToken, Task<StatefulParser?>> receiving,
        IStatefulParsersStorage storage,
        CancellationToken ct = default)
    {
        StatefulParser? parser = await receiving(storage, ct);
        if (parser == null) return Error.NotFound("Парсер не найден.");
        return parser;
    }

    public StatefulParser AddListener(IOnStatefulParserStateChangedEventListener listener)
    {
        return new StatefulParser(this, listener);
    }
    
    private StatefulParser(StatefulParser parser, IOnStatefulParserStateChangedEventListener listener)
    : this(parser._registered, parser._workTurner)
    {
        _listener = listener;
    }
    
    private async Task<Result<Unit>> UpdateState(
        Func<ParserWorkTurner, Task<Result<ParserWorkTurner>>> stateChangeFn,
        CancellationToken ct
        )
    {
        Result<ParserWorkTurner> update = await stateChangeFn(_workTurner);
        if (update.IsFailure) return update.Error;
        Result<Unit> react = await _listener.React(_registered, update, ct);
        if (react.IsFailure) return react.Error;
        return Unit.Value;
    }
}