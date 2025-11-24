using RemTech.SharedKernel.Core.PrimitivesModule.Exceptions;

namespace RemTech.SharedKernel.Core.FunctionExtensionsModule;

public sealed class ErrorExceptionToResultAdapter(ErrorException ex)
{
    public Result Adapt()
    {
        return ex switch
        {
            ErrorException.ValidationException => Result.Failure(Error.Validation(ex.Error)),
            ErrorException.ConflictException => Result.Failure(Error.Conflict(ex.Error)),
            ErrorException.InternalException => Result.Failure(Error.Application(ex.Error)),
            ErrorException.NotFoundException => Result.Failure(Error.NotFound(ex.Error)),
            _ => throw new ApplicationException($"Unable to resolve error type: {ex.GetType().Name}")
        };
    }
}