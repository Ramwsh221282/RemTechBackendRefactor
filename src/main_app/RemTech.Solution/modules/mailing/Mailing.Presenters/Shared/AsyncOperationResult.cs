using RemTech.Functional.Extensions;
using RemTech.Primitives.Extensions.Exceptions;

namespace Mailing.Presenters.Shared;

public sealed class AsyncOperationResult(Func<Task> func)
{
    public async Task<Result<Unit>> Process()
    {
        try
        {
            await func();
            return Result.Success(Unit.Value);
        }
        catch(ErrorException ex)
        {
            Result result = new ErrorExceptionToResultAdapter(ex).Adapt();
            return Result.Failure<Unit>(result.Error);
        }
    }
}

public sealed class AsyncOperationResult<T>(Func<Task<T>> func)
{
    public async Task<Result<T>> Process()
    {
        try
        {
            T result = await func();
            return Result.Success(result);
        }
        catch (ErrorException ex)
        {
            Result result = new ErrorExceptionToResultAdapter(ex).Adapt();
            return Result.Failure<T>(result.Error);
        }
    }
}