using Mailing.Presenters.Mailers.AddMailer;
using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.Mailing.Features;

public sealed class AddMailerFeature(IServiceProvider sp)
{
    public async Task<Result<AddMailerResponse>> Invoke(string email, string password)
    {
        CancellationToken ct = CancellationToken.None;
        AddMailerRequest request = new(password, email, ct);
        await using AsyncServiceScope scope = sp.CreateAsyncScope();
        IGateway<AddMailerRequest, AddMailerResponse> service = scope.Resolve<IGateway<AddMailerRequest, AddMailerResponse>>();
        return await service.Execute(request);
    }
}