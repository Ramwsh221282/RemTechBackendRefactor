using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.ParsedVehicles.ParsedVehicleKinds;
using Parsing.Vehicles.Common.TextWriting;
using Parsing.Vehicles.Grpc.Recognition;
using Parsing.Vehicles.Grpc.Recognition.VehicleKind;
using PuppeteerSharp;

namespace Avito.Parsing.Vehicles.VehiclesParsing.AvitoVehicleAttributeSources.Kind;

public sealed class GrpcVehicleKindFromTitle(CommunicationChannel channel, IPage page) : IParsedVehicleKindSource
{
    private readonly string _titleSelector = string.Intern("h1[data-marker='item-view/title-info']");
    
    public async Task<ParsedVehicleKind> Read()
    {
        string text = await new TextFromWebElement(await new PageElementSource(page).Read(_titleSelector)).Read();
        text = text.Replace(",", string.Empty).Trim();
        Characteristic ctx = await new VehicleKindRecognition(channel).Recognize(text);
        return ctx
            ? new ParsedVehicleKind(ctx.ReadValue())
            : throw new ArgumentException("Vehicle kind was not recognized through grpc recognizer service.");
    }
}