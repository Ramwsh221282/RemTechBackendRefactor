using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

public interface IAsyncOperationResult
{
    Task<Result<Unit>> Process();
}

public interface IAsyncOperationResult<T>
{
    Task<Result<T>> Process();
}

public sealed class AsyncOperationResult(Func<Task> func) : IAsyncOperationResult
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

public sealed class AsyncOperationResult<T>(Func<Task<T>> func) : IAsyncOperationResult<T>
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