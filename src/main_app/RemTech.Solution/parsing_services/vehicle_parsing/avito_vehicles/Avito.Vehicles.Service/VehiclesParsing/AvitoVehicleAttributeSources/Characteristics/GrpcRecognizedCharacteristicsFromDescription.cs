using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleCharacteristics;
using Parsing.Vehicles.Grpc.Recognition;
using Parsing.Vehicles.Grpc.Recognition.BucketCapacity;
using Parsing.Vehicles.Grpc.Recognition.BucketControlType;
using Parsing.Vehicles.Grpc.Recognition.BuRecognition;
using Parsing.Vehicles.Grpc.Recognition.EngineModel;
using Parsing.Vehicles.Grpc.Recognition.EnginePower;
using Parsing.Vehicles.Grpc.Recognition.EngineType;
using Parsing.Vehicles.Grpc.Recognition.EngineVolume;
using Parsing.Vehicles.Grpc.Recognition.FuelTankCapacity;
using Parsing.Vehicles.Grpc.Recognition.LoadingHeight;
using Parsing.Vehicles.Grpc.Recognition.LoadingWeight;
using Parsing.Vehicles.Grpc.Recognition.ReleaseYear;
using Parsing.Vehicles.Grpc.Recognition.Torque;
using Parsing.Vehicles.Grpc.Recognition.TransportHours;
using Parsing.Vehicles.Grpc.Recognition.UnloadingHeight;
using Parsing.Vehicles.Grpc.Recognition.Vin;
using Parsing.Vehicles.Grpc.Recognition.Weight;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class GrpcRecognizedCharacteristicsFromDescription(
    IPage page,
    ICommunicationChannel channel
) : IKeyValuedCharacteristicsSource
{
    public async Task<CharacteristicsDictionary> Read()
    {
        return new CharacteristicsDictionary();
    }

    public async Task<IEnumerable<VehicleCharacteristic>> Recognize(string text)
    {
        DictionariedRecognitions recognitions = await new DictionariedRecognitions()
            .With(new BucketCapacityRecognition(channel))
            .With(new BucketControlTypeRecognition(channel))
            .With(new BuRecognition(channel))
            .With(new EngineModelRecognition(channel))
            .With(new EnginePowerRecognition(channel))
            .With(new EngineTypeRecognition(channel))
            .With(new EngineVolumeRecognition(channel))
            .With(new FuelTankCapacityRecognition(channel))
            .With(new LoadingHeightRecognition(channel))
            .With(new LoadingWeightRecognition(channel))
            .With(new ReleaseYearRecognition(channel))
            .With(new TorqueRecognition(channel))
            .With(new TransportHoursRecognition(channel))
            .With(new UnloadingHeightRecognition(channel))
            .With(new VinRecognition(channel))
            .With(new WeightRecognition(channel))
            .Processed(text);
        return recognitions
            .All()
            .Select(r => new VehicleCharacteristic(r.ReadName(), r.ReadValue()));
    }
}
