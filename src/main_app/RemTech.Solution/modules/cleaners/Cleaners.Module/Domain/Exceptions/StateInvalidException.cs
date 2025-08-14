namespace Cleaners.Module.Domain.Exceptions;

internal sealed class StateInvalidException : Exception
{
    public StateInvalidException(string state)
        : base($"Состояние чистильщика: {state} не допустимо.") { }
}
