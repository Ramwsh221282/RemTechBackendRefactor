using Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.Grpc.Recognition;
using Parsing.Vehicles.Grpc.Recognition.VehicleKind;
using PuppeteerSharp;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class GrpcVehicleKindFromDescription : IParsedVehicleKindSource
{
    private readonly CommunicationChannel _channel;
    private readonly IPage _page;

    public GrpcVehicleKindFromDescription(CommunicationChannel channel, IPage page)
    {
        _channel = channel;
        _page = page;
    }

    public async Task<ParsedVehicleKind> Read()
    {
        string[] textParts = await new EmptyOnErrorDescriptionParts(new AvitoDescriptionParts(_page)).Read();
        VehicleKindRecognition recognition = new(_channel);
        foreach (string text in textParts)
        {
            Characteristic kind = await recognition.Recognize(text);
            if (kind)
                return new ParsedVehicleKind(kind.ReadValue());
        }

        throw new ArgumentException(
            "Vehicle kind from description was not recognized through grpc recognizer service.");
    }
}