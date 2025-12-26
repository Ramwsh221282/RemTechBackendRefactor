namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

public interface IFunction<in TArgument, out TReturn> where TArgument : IFunctionArgument
{
    TReturn Invoke(TArgument argument);
}