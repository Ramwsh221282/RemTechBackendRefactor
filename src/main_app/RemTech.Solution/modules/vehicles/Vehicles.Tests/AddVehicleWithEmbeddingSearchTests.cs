using Microsoft.Extensions.DependencyInjection;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using Vehicles.Domain.Features.AddVehicle;

namespace Vehicles.Tests;

public sealed class AddVehicleWithEmbeddingSearchTests(NoContainersIntegrationalTestsFixture fixture) : IClassFixture<NoContainersIntegrationalTestsFixture>
{
    private IServiceProvider Services { get; } = fixture.Services;

    [Fact]
    private async Task Add_Vehicles_Success()
    {
        await using AsyncServiceScope scope = Services.CreateAsyncScope();
        ICommandHandler<AddVehicleCommand, Unit> handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<AddVehicleCommand, Unit>>();
        
        AddVehicleCreatorCommandPayload creator = new(Guid.NewGuid(), "Test", "Test");
        AddVehicleVehiclesCommandPayload vehicle = new(
            Id: Guid.NewGuid(),
            Title: "Харверстерная головка John Deere H480C",
            Url: "https://example.com",
            Price: 100000,
            IsNds: false,
            Address: "Ленинградская обл., Всеволожский р-н, Куйвозовское сельское поселение, пос. Стеклянный, Заводская ул., 7",
            Photos: ["https://example.com/photo1.jpg", "https://example.com/photo2.jpg"],
            Characteristics: 
            [
                new AddVehicleCommandCharacteristics("Мощность", "100 л.с."),
                new AddVehicleCommandCharacteristics("Грузоподъемность", "1000 кг"),
                new AddVehicleCommandCharacteristics("Год выпуска", "2013"),
            ]);
        
        AddVehicleCommand command = new(creator, [vehicle]);
        Result<Unit> unit = await handler.Execute(command);
        Assert.True(unit.IsSuccess);
    }
}