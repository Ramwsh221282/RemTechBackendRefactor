namespace RemTech.Functional.Extensions;

public interface IFunction<in TArgument, out TReturn> where TArgument : IFunctionArgument
{
    TReturn Invoke(TArgument argument);
}