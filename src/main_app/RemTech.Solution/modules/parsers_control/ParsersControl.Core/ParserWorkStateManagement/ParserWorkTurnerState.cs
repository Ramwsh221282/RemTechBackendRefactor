using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkStateManagement;

public sealed record ParserWorkTurnerState(Guid Id, ParserState State)
{
    public Result<ParserWorkTurnerState> SwitchState(ParserState state)
    {
        Result<ParserState> @switch = State.SwitchTo(state); 
        return @switch.IsFailure ? @switch.Error : this with { State = state };
    }

    public void Write(
        Action<Guid>? writeId = null,
        Action<string>? writeState = null
        )
    {
        writeId?.Invoke(Id);
        writeState?.Invoke(State.ToString());
    }
}