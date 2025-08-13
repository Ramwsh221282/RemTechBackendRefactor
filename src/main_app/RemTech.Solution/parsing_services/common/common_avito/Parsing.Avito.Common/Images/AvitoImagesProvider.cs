using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Parsing.Avito.Common.Images;

public sealed class AvitoImagesProvider(IElementHandle item)
{
    private const string SliderSelector = ".photo-slider-list-R0jle";

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
                string attribute = await new AttributeFromWebElement(image, "data-marker").Read();
                if (!attribute.Contains("image"))
                    continue;
                if (string.IsNullOrWhiteSpace(attribute))
                    continue;
                string photo = attribute.Replace("slider-image/image-", string.Empty);
                images.AddFirst(photo);
            }
            catch { }
        }

        return images;
    }
}
