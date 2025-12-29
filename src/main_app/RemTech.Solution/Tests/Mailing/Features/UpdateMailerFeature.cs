using Mailing.Presenters.Mailers.UpdateMailer;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Mailing.Features;

public sealed class UpdateMailerFeature(IServiceProvider sp)
{
    public async Task<Result<UpdateMailerResponse>> Invoke(Guid mailerId, string newEmail, string newPassword)
    {
        CancellationToken ct = CancellationToken.None;
        UpdateMailerRequest request = new(mailerId, newEmail, newPassword, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<UpdateMailerRequest, UpdateMailerResponse> service =
            scope.Resolve<IGateway<UpdateMailerRequest, UpdateMailerResponse>>();
        return await service.Execute(request);
    }
}