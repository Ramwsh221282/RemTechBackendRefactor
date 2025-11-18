using CompositionRoot.Shared;
using Mailers.Application.Features.ChangeMailerSmtpPassword;
using Mailers.Application.Features.CreateMailer;
using Mailers.Application.Features.DeleteMailer;
using Mailers.Application.Features.PingMailer;
using Microsoft.Extensions.DependencyInjection;

namespace CompositionRoot.mailing.DependencyInjection.Features;

[DependencyInjectionClass]
internal static class MailingFeaturesInjection
{
    [DependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<ChangeMailerSmtpPasswordUseCase>();
        services.AddScoped<CreateMailerUseCase>();
        services.AddScoped<DeleteMailerUseCase>();
        services.AddScoped<PingMailerUseCase>();
    }
}