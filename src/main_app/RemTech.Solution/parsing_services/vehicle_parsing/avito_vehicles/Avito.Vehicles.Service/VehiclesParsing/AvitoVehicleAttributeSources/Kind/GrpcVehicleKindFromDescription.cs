using Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.Grpc.Recognition;
using Parsing.Vehicles.Grpc.Recognition.VehicleKind;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class GrpcVehicleKindFromDescription(ICommunicationChannel channel, IPage page)
    : IParsedVehicleKindSource
{
    public async Task<ParsedVehicleKind> Read()
    {
        string[] textParts = await new EmptyOnErrorDescriptionParts(
            new AvitoDescriptionParts(page)
        ).Read();
        VehicleKindRecognition recognition = new(channel);
        foreach (string text in textParts)
        {
            Characteristic kind = await recognition.Recognize(text);
            if (kind)
                return new ParsedVehicleKind(kind.ReadValue());
        }

        throw new ArgumentException(
            "Vehicle kind from description was not recognized through grpc recognizer service."
        );
    }
}
