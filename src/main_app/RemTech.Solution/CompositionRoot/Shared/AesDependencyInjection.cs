using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Infrastructure.AesEncryption;

namespace CompositionRoot.Shared;

[DependencyInjectionClass]
public static class AesDependencyInjection
{
    [DependencyInjectionMethod]
    public static void AddQuartzJobs(this IServiceCollection services)
    {
        services.AddAesCryptography();
    }
}