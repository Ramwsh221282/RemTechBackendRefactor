using Parsing.SDK.ScrapingArtifacts;
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
using RemTech.Core.Shared.Primitives;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public sealed class GrpcRecognizedCharacteristics : IKeyValuedCharacteristicsSource
{
    private readonly IPage _page;
    private readonly CommunicationChannel _channel;

    public GrpcRecognizedCharacteristics(IPage page, CommunicationChannel channel)
    {
        _page = page;
        _channel = channel;
    }
    
    public async Task<CharacteristicsDictionary> Read()
    {
        IElementHandle[] ctxes = await new DefaultOnErrorAvitoCharacteristics(
                new AvitoCharacteristicsSource(_page))
            .Read();
        string ctxRawText = string.Join(' ', await CharacteristicTexts(ctxes));
        CharacteristicsDictionary dictionary = new CharacteristicsDictionary();
        DictionariedRecognitions recognitions = await new DictionariedRecognitions()
            .With(new MeasuringBucketCapacityRecognition(new BucketCapacityRecognition(_channel)))
            .With(new MeasurementBucketControlTypeRecognition(new BucketControlTypeRecognition(_channel)))
            .With(new BuRecognition(_channel))
            .With(new EngineModelRecognition(_channel))
            .With(new MeasuringEnginePowerRecognition(new EnginePowerRecognition(_channel)))
            .With(new EngineTypeRecognition(_channel))
            .With(new EngineVolumeRecognition(_channel))
            .With(new FuelTankCapacityRecognition(_channel))
            .With(new LoadingHeightRecognition(_channel))
            .With(new LoadingWeightRecognition(_channel))
            .With(new ReleaseYearRecognition(_channel))
            .With(new TorqueRecognition(_channel))
            .With(new TransportHoursRecognition(_channel))
            .With(new UnloadingHeightRecognition(_channel))
            .With(new VinRecognition(_channel))
            .With(new WeightRecognition(_channel))
            .Processed(ctxRawText);
        foreach (var recognition in recognitions.All())
            dictionary.With(new VehicleCharacteristic(recognition.ReadName(), recognition.ReadValue()));
        return dictionary;
    }

    private async Task<string[]> CharacteristicTexts(IElementHandle[] ctxes)
    {
        string[] array = new string[ctxes.Length];
        int index = 0;
        foreach (IElementHandle ctxe in ctxes)
        {
            string pair = await new TextFromWebElement(ctxe).Read();
            pair = pair.Replace(":", string.Empty).Trim();
            array[index] = pair;
            index++;
        }

        return array;
    }
}