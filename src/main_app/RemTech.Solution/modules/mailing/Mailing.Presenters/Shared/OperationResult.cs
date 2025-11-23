using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions.Exceptions;

namespace Mailing.Presenters.Shared;

public sealed class OperationResult(Action action)
{
    public Result Process()
    {
        try
        {
            action();
            return Result.Success();
        }
        catch (ErrorException ex)
        {
            return new ErrorExceptionToResultAdapter(ex).Adapt();
        }
    }
}

public sealed class OperationResult<T>(Func<T> func)
{
    public Result<T> Process()
    {
        try
        {
            T result = func();
            return Result.Success(result);
        }
        catch (ErrorException ex)
        {
            Result result = new ErrorExceptionToResultAdapter(ex).Adapt();
            return Result.Failure<T>(result.Error);
        }
    }
}