using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RemTech.Ner.VehicleParameters;

namespace ParsedAdvertisements.Tests;

public sealed class ParsedAdvertisementsTestsService : IAsyncLifetime
{
    private const string modelPath = "./VehicleNer";
    
    private readonly Lazy<IServiceProvider> _sp;
    
    private IServiceProvider Sp => _sp.Value;

    public AsyncServiceScope Scope => Sp.CreateAsyncScope();
    

    public ParsedAdvertisementsTestsService()
    {
        _sp = new Lazy<IServiceProvider>(InitializeServices);
    }

    public async Task InitializeAsync()
    {
        
    }

    public async Task DisposeAsync()
    {
        
    }
    
    private IServiceProvider InitializeServices()
    {
        ServiceCollection services = new();
        
        IOptions<VehicleParametersNerOptions> options = Options.Create(new VehicleParametersNerOptions()
        {
            Id2LabelPath = AddFileToPath("id2label.json"),
            ModelPath = AddFileToPath("quantized_model.onnx"),
            VocabPath = AddFileToPath("vocab.txt")
        });
        
        services.AddSingleton(options);
        services.AddVehicleParametersNer();
        return services.BuildServiceProvider();
    }

    private static string AddFileToPath(string file)
    {
        return Path.Combine(modelPath, file);
    }
}