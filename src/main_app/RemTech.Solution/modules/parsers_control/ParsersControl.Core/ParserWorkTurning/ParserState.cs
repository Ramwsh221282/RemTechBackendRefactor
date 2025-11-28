using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserWorkTurning;

public abstract record ParserState(string Value)
{
    public sealed record Enabled() : ParserState("Включен")
    {
        public override Result<ParserState> SwitchTo(ParserState state)
        {
            return state switch
            {
                Enabled => Error.Conflict("Парсер уже включен"),
                Working => Error.Conflict("Парсер уже включен"),
                _ => state
            };
        }
    }

    public sealed record Disabled() : ParserState("Выключен")
    {
        public override Result<ParserState> SwitchTo(ParserState state)
        {
            return state switch
            {
                Disabled => Error.Conflict("Парсер уже выключен"),
                Waiting => Error.Conflict("Парсер должен быть включен"),
                Working => Error.Conflict("Парсер должен быть включен"),
                _ => state
            };
        }
    }

    public sealed record Working() : ParserState("Работает")
    {
        public override Result<ParserState> SwitchTo(ParserState state)
        {
            return state switch
            {
                Enabled => Error.Conflict("Парсер уже включен"),
                Working => Error.Conflict("Парсер уже включен"),
                _ => state
            };
        }
    }

    public sealed record Waiting() : ParserState("Ожидает")
    {
        public override Result<ParserState> SwitchTo(ParserState state)
        {
            return state switch
            {
                Waiting => Error.Conflict("Парсер уже в ожидании"),
                Enabled => Error.Conflict("Парсер уже работает"),
                _ => state
            };
        }
    }

    public abstract Result<ParserState> SwitchTo(ParserState state);
}