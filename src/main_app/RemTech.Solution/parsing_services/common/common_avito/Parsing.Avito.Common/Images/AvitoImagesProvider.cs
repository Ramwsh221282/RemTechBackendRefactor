using System.Text.RegularExpressions;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Parsing.Avito.Common.Images;

public sealed partial class AvitoImagesProvider(IElementHandle item)
{
    private const string SliderSelector = ".photo-slider-list-R0jle";
    private const StringSplitOptions SplitOptions =
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
    private static Regex _srcSetRegex = MyRegex();

    public async Task<IEnumerable<string>> GetImages()
    {
        LinkedList<string> images = [];
        IElementHandle slider = await new ValidSingleElementSource(
            new ParentElementSource(item)
        ).Read(SliderSelector);
        IElementHandle[] elements = await new ParentManyElementsSource(slider).Read("li");
        foreach (var image in elements)
        {
            try
            {
                string srcSet = await GetSrcSet(image);
                if (string.IsNullOrEmpty(srcSet))
                    continue;
                string highQualityImage = srcSet.Split(',', SplitOptions)[^1].Split(' ')[0].Trim();
                images.AddFirst(highQualityImage);
            }
            catch { }
        }

        return images;
    }

    private static async Task<string> GetSrcSet(IElementHandle element)
    {
        string innerHtml = await element.EvaluateFunctionAsync<string>("el => el.innerHTML");
        if (string.IsNullOrWhiteSpace(innerHtml))
            return string.Empty;
        Match match = _srcSetRegex.Match(innerHtml);
        if (!match.Success)
            return string.Empty;
        if (match.Groups.Count < 2)
            return string.Empty;
        return match.Groups[1].Value;
    }

    [GeneratedRegex(@"srcset=[""](.+)[""]\ssizes", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
