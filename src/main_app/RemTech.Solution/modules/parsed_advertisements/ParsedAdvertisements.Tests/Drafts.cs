using Microsoft.Extensions.DependencyInjection;
using RemTech.Ner.VehicleParameters;

namespace ParsedAdvertisements.Tests;

public sealed class Drafts(ParsedAdvertisementsTestsService service) : IClassFixture<ParsedAdvertisementsTestsService>
{
    [Fact]
    private async Task Classify_Input_No_Exceptions()
    {
        string input = "Продается вилочный погрузчик John Deere MK5123-3212";
        await using AsyncServiceScope scope = service.Scope;
        IVehicleNerService nerService = scope.ServiceProvider.GetRequiredService<IVehicleNerService>();
        IReadOnlyList<VehicleNerOutput> detectedParameters = nerService.DetectParameters(input);
        foreach (VehicleNerOutput detectedParameter in detectedParameters)
        {
            Console.WriteLine($"Vehicle param: {detectedParameter.Word}. Detected: {detectedParameter.Label}. Confidence: {detectedParameter.Confidence}");
        }

        int a = 0;
    }
}