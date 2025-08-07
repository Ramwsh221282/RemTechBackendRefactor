using System.Text.RegularExpressions;
using Parsing.SDK.ElementSources;
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

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

public interface IAvitoDescriptionParts
{
    Task<string[]> Read();
}

public sealed class EmptyOnErrorDescriptionParts : IAvitoDescriptionParts
{
    private readonly IAvitoDescriptionParts _origin;

    public EmptyOnErrorDescriptionParts(IAvitoDescriptionParts origin)
    {
        _origin = origin;
    }

    public async Task<string[]> Read()
    {
        try
        {
            return await _origin.Read();
        }
        catch
        {
            return [];
        }
    }
}

public sealed class AvitoDescriptionParts : IAvitoDescriptionParts
{
    private readonly IPage _page;
    private readonly string _descriptionSelector = string.Intern("#bx_item-description");

    public AvitoDescriptionParts(IPage page)
    {
        _page = page;
    }

    public async Task<string[]> Read()
    {
        IElementHandle descriptionElement = await new PageElementSource(_page).Read(
            _descriptionSelector
        );
        HashSet<string> texts = [];
        texts.UnionWith(await FromParagraphs(descriptionElement));
        texts.UnionWith(await FromBrs(descriptionElement));
        return texts.ToArray();
    }

    public async Task<string[]> FromParagraphs(IElementHandle descriptionContainer)
    {
        IElementHandle[] paragraphs = await new ParentManyElementsSource(descriptionContainer).Read(
            "p"
        );
        string[] texts = new string[paragraphs.Length];
        for (int i = 0; i < texts.Length; i++)
            texts[i] = await PreprocessedText(paragraphs[i]);
        return texts;
    }

    public async Task<string[]> FromBrs(IElementHandle descriptionElement)
    {
        IElementHandle? paragraphContainer = await new ParentElementSource(descriptionElement).Read(
            "p"
        );
        if (paragraphContainer == null)
            return [];

        IElementHandle[] textParts = await new ParentManyElementsSource(paragraphContainer).Read(
            "br"
        );
        string[] formatted = new string[textParts.Length];
        for (int i = 0; i < textParts.Length; i++)
            formatted[i] = await PreprocessedText(textParts[i]);
        return formatted;
    }

    private async Task<string> PreprocessedText(IElementHandle textPart)
    {
        string text = await new TextFromWebElement(textPart).Read();
        return await PreprocessedText(text);
    }

    private Task<string> PreprocessedText(string text)
    {
        string formatted = text.Replace("\"", " ")
            .Replace(".", " ")
            .Replace(",", " ".Replace("/", " "))
            .Replace(":", " ");
        formatted = Regex.Replace(formatted, @"\s+", " ");
        return Task.FromResult(formatted.Trim());
    }
}

public sealed class GrpcRecognizedCharacteristicsFromDescription(
    IPage page,
    ICommunicationChannel channel
) : IKeyValuedCharacteristicsSource
{
    public async Task<CharacteristicsDictionary> Read()
    {
        string[] textParts = await new EmptyOnErrorDescriptionParts(
            new AvitoDescriptionParts(page)
        ).Read();
        CharacteristicsDictionary dictionary = new CharacteristicsDictionary();
        foreach (string text in textParts)
        {
            dictionary = dictionary.FromEnumerable(await Recognize(text));
        }

        return dictionary;
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
