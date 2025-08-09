using Parsing.Vehicles.Grpc.Recognition.BucketCapacity;
using Parsing.Vehicles.Grpc.Recognition.BucketControlType;
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

namespace Parsing.Vehicles.Grpc.Recognition;

public sealed class CharacteristicsRecognitionFromText(ICommunicationChannel channel)
{
    public async Task<Dictionary<string, string>> Recognize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return [];
        DictionariedRecognitions recognitions = await new DictionariedRecognitions()
            .With(new BucketCapacityRecognition(channel))
            .With(new BucketControlTypeRecognition(channel))
            .With(new BuRecognition.BuRecognition(channel))
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
        Dictionary<string, string> characteristicsDictionary = [];
        foreach (var characteristic in recognitions.All())
        {
            string name = characteristic.ReadName();
            string value = characteristic.ReadValue();
            if (!characteristicsDictionary.ContainsKey(name))
                characteristicsDictionary.Add(name, value);
        }

        return characteristicsDictionary;
    }
}
