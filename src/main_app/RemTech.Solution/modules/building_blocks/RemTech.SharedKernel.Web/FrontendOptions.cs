using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace RemTech.SharedKernel.Web;

public sealed class FrontendOptions
{
    public string FrontendUrl { get; set; } = string.Empty;

    public static void AddFrontendOptions(IServiceCollection services)
    {
        services.AddOptions<FrontendOptions>().BindConfiguration(nameof(FrontendOptions));
    }

    public static void ReconfigureOptions(IServiceCollection services, FrontendOptions reconfigured)
    {
        services.RemoveAll<IOptions<FrontendOptions>>();
        IOptions<FrontendOptions> options = Options.Create(reconfigured);
        services.AddSingleton(options);
    }
}