using Identity.Contracts.Permissions;
using Identity.Contracts.Permissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.Permissions;

public class AsyncPermissionDataProcessingPipelineMethod
{
    private readonly Queue<Func<PermissionData, CancellationToken, Task<Result<Unit>>>> _operations = [];

    public void Add(object processor)
    {
        Action adding = processor switch
        {
            IOnPermissionCreatedEventListener created => () => _operations.Enqueue(created.React),
            IOnPermissionRenamedEventListener renamed => () => _operations.Enqueue(renamed.React),
            _ => throw new InvalidOperationException($"Instance: {processor.GetType().Name} is not event listener.")
        };
        adding();
    }

    public async Task<Result<Unit>> Process(PermissionData data, CancellationToken ct = default)
    {
        while (_operations.Count > 0)
        {
            Func<PermissionData, CancellationToken, Task<Result<Unit>>> operation = _operations.Dequeue();
            Result<Unit> processing = await operation(data, ct);
            if (processing.IsFailure) return processing.Error;
        }

        return Unit.Value;
    }
}