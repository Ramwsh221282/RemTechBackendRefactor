using RemTech.Result.Pattern;
using Vehicles.Domain.VehicleContext;
using Vehicles.Tests.Configurations;
using Vehicles.UseCases.AddVehicle;

namespace Vehicles.Tests.VehicleContext;

public sealed class AddVehicleTests : IClassFixture<VehiclesApplicationFactory>
{
    private readonly VehicleTestsHelper _vehicles;

    public AddVehicleTests(VehiclesApplicationFactory factory)
    {
        _vehicles = new VehicleTestsHelper(factory);
    }

    [Fact]
    private async Task Add_Vehicle_Success_Test()
    {
        AddVehicleCommand command = FormValidCommand();
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsSuccess);
        Result<Vehicle> created = await _vehicles.GetVehicleById(vehicle.Value.Id);
        Assert.True(created.IsSuccess);
    }

    [Fact]
    private async Task Add_Vehicle_Empty_Category_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with { CategoryName = "" };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Empty_Brand_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with { BrandName = "" };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Empty_Model_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with { ModelName = "" };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Empty_Location_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with { LocationParts = [] };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Repeatable_Locations_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with { LocationParts = ["Moscow", "Moscow"] };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Empty_Location_Name_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with { LocationParts = ["", "Moscow"] };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Empty_Description_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with { Description = "" };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Empty_Characteristics_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with { Characteristics = [] };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Repeatable_Characteristics_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with
        {
            Characteristics =
            [
                new AddVehicleCommandCharacteristic("Release Year", "2021"),
                new AddVehicleCommandCharacteristic("Release Year", "2024"),
            ],
        };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Invalid_Ctx_Name_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with
        {
            Characteristics =
            [
                new AddVehicleCommandCharacteristic("", "2021"),
                new AddVehicleCommandCharacteristic("Release Year", "2024"),
            ],
        };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Invalid_Ctx_Value_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with
        {
            Characteristics =
            [
                new AddVehicleCommandCharacteristic("Release Year", ""),
                new AddVehicleCommandCharacteristic("Release Year", "2024"),
            ],
        };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Empty_Photos_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with { PhotoPaths = [] };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    [Fact]
    private async Task Add_Vehicle_Empty_Photo_Path_Failure()
    {
        AddVehicleCommand command = FormValidCommand();
        command = command with { PhotoPaths = ["ddsadasdsadas", ""] };
        Result<Vehicle> vehicle = await _vehicles.AddVehicle(command);
        Assert.True(vehicle.IsFailure);
    }

    private AddVehicleCommand FormValidCommand()
    {
        string category = "Легковые автомобили";
        string brand = "Toyota";
        string model = "Camry";
        IEnumerable<string> locations = ["Москва", "Центр"];
        string description = "Отличное состояние, один хозяин, полный комплект";
        long price = 2500000;
        bool isNds = false;
        IEnumerable<AddVehicleCommandCharacteristic> characteristics =
        [
            new("Год выпуска", "2022"),
            new("Пробег", "15000 км"),
            new("Цвет", "Серебристый"),
            new("КПП", "Автомат"),
        ];
        IEnumerable<string> photoPaths = ["photo_1", "photo_2"];
        return new AddVehicleCommand(
            category,
            brand,
            model,
            locations,
            description,
            new AddVehicleCommandPriceInfo(price, isNds),
            characteristics,
            photoPaths
        );
    }
}
