using System.Text.RegularExpressions;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Avito.Vehicles.Service.VehiclesParsing.AvitoVehicleAttributeSources.Characteristics;

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
