using Mailers.Application.Features.ChangeMailerSmtpPassword;
using Mailers.Application.Features.CreateMailer;
using Mailers.Application.Features.DeleteMailer;
using Mailers.Application.Features.PingMailer;
using Microsoft.Extensions.DependencyInjection;

namespace Mailers.Application;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddMailersApplication()
        {
            services.AddScoped<ChangeMailerSmtpPasswordUseCase>();
            services.AddScoped<CreateMailerUseCase>();
            services.AddScoped<DeleteMailerUseCase>();
            services.AddScoped<PingMailerUseCase>();
        }
    }
}