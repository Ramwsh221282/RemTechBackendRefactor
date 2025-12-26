using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Ner.VehicleParameters;

public static class DependencyInjection
{
    extension(IServiceCollection services)
    {
        public void AddVehicleParametersNer()
        {
            services.AddSingleton<VehicleNerModelMetadata>();
            services.AddSingleton<NerModelInference>();
            services.AddSingleton<IVehicleNerService, VehicleNerService>();
        }
    }
}