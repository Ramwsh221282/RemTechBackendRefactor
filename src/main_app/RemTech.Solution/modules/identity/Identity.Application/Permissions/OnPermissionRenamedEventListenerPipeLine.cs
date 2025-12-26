using Identity.Contracts.Permissions;
using Identity.Contracts.Permissions.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace Identity.Application.Permissions;

public sealed class OnPermissionRenamedEventListenerPipeLine : IOnPermissionRenamedEventListener
{
    private readonly AsyncPermissionDataProcessingPipelineMethod _pipeLine = new();

    public OnPermissionRenamedEventListenerPipeLine Add(IOnPermissionRenamedEventListener listener)
    {
        _pipeLine.Add(listener);
        return this;
    }

    public async Task<Result<Unit>> React(PermissionData data, CancellationToken ct = default) =>
        await _pipeLine.Process(data, ct);
}