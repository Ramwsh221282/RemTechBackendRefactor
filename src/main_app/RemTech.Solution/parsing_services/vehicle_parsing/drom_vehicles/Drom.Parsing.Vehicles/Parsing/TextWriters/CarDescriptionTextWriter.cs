using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.TextWriting;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.TextWriters;

public sealed class CarDescriptionTextWriter(ITextWrite write, IPage page)
{
    private readonly string _containerSelector = string.Intern(".css-inmjwf.e162wx9x0");
    private readonly string _innerContainerSelector = string.Intern(".css-1kb7l9z.e162wx9x0");

    public async Task Write()
    {
        try
        {
            IElementHandle container = await new ValidSingleElementSource(
                new PageElementSource(page)
            ).Read(_containerSelector);
            IElementHandle innerContainer = await new ValidSingleElementSource(
                new ParentElementSource(container)
            ).Read(_innerContainerSelector);
            string text = await new TextFromWebElement(innerContainer).Read();
            await write.WriteAsync(text);
        }
        catch
        {
            Console.WriteLine("Unable to write text.");
        }
    }
}
