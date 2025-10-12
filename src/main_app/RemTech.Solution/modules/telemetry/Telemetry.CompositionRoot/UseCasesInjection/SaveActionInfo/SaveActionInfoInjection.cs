using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RemTech.DependencyInjection;
using RemTech.UseCases.Shared.Cqrs;
using Telemetry.Domain.TelemetryContext;
using Telemetry.UseCases.SaveActionInfo;

namespace Telemetry.CompositionRoot.UseCasesInjection.SaveActionInfo;

/// <summary>
/// Инъекция обработчика добавления действия
/// </summary>
[InjectionClass]
public static class SaveActionInfoInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<IValidator<SaveActionInfoIbCommand>, SaveActionInfoCommandValidator>();
        services.AddScoped<
            ICommandHandler<SaveActionInfoIbCommand, TelemetryRecord>,
            SaveActionInfoIbCommandHandler
        >();
    }
}
