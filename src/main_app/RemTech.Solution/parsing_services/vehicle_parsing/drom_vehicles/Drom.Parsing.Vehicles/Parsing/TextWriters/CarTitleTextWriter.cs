using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using Parsing.Vehicles.Common.TextWriting;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.TextWriters;

public sealed class CarTitleTextWriter(ITextWrite write, IPage page)
{
    private readonly string _titleSelector = string.Intern(".ftldj64.css-uewl2b");
    private readonly string _titleContainerSelector = string.Intern(".css-987tv1.eotelyr0");

    public async Task Write()
    {
        IElementHandle titleContainer = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(_titleSelector);
        IElementHandle title = await new ValidSingleElementSource(
            new ParentElementSource(titleContainer)
        ).Read(_titleContainerSelector);
        IElementHandle h1 = await new ValidSingleElementSource(new ParentElementSource(title)).Read(
            string.Intern("h1")
        );
        IElementHandle span = await new ValidSingleElementSource(new ParentElementSource(h1)).Read(
            string.Intern("span")
        );
        string text = await new TextFromWebElement(span).Read();
        string formatted = text.Replace(",", string.Empty);
        await write.WriteAsync(formatted);
    }
}
