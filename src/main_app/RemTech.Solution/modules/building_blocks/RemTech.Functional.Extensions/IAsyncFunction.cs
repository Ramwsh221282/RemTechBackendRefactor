namespace RemTech.Functional.Extensions;

public interface IAsyncFunction<in TArgument, TReturn> where TArgument : IFunctionArgument
{
    Task<TReturn> Invoke(TArgument argument, CancellationToken ct);
}