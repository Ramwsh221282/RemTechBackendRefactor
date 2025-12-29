using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace Tests.ParsersControl.Features;

public sealed class AddParserFeature(IServiceProvider sp)
{
    public async Task<Result<AddParserResponse>> Invoke(string domain, string type)
    {
        CancellationToken ct = CancellationToken.None;
        return await
            sp.ExecuteInScope<
                IGateway<AddParserRequest, AddParserResponse>, 
                Result<AddParserResponse>
                >
            (gateway => gateway.Execute(new AddParserRequest(domain, type, ct)));
    }
}