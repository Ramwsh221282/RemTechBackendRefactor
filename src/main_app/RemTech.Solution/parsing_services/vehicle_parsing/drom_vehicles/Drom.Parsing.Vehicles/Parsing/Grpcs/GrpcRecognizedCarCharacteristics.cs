using System.Text.RegularExpressions;
using Drom.Parsing.Vehicles.Parsing.Models;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Grpc.Recognition;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.Grpcs;

public sealed class GrpcRecognizedCarCharacteristics(IPage page, ICommunicationChannel channel)
{
    public async Task Print(DromCatalogueCar car)
    {
        IElementHandle firstContainer = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(".css-inmjwf.e162wx9x0");
        string text = await new TextFromWebElement(firstContainer).Read();
        string formatted = text.Replace("Дополнительно: ", string.Empty);
        formatted = Regex.Replace(formatted, @"\r\n?|\n", " ");
        formatted = Regex.Replace(formatted, @"\s+", " ").Trim();
        Console.WriteLine(formatted);
        CharacteristicsRecognitionFromText recognition = new(channel);
        Dictionary<string, string> dictionary = await recognition.Recognize(formatted);
        foreach (KeyValuePair<string, string> pair in dictionary)
            car.WithCharacteristic(pair.Key, pair.Value);
    }
}
