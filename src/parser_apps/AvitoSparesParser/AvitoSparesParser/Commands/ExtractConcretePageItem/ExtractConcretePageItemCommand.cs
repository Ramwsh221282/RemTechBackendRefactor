using AvitoSparesParser.AvitoSpareContext;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace AvitoSparesParser.Commands.ExtractConcretePageItem;

public sealed class ExtractConcretePageItemCommand(
    IPage browserPage,
    AvitoBypassFactory bypassFactory
) : IExtractConcretePageItemCommand
{
    private static readonly NavigationOptions _options = new()
    {
        Timeout = 3000,
        WaitUntil = [WaitUntilNavigation.Load]
    };

    public async Task<AvitoSpare> Extract(AvitoSpare spare)
    {
        const string javaScript =
            @"
        () => {
    const title = document.querySelector('h1[itemprop=""name""]')?.innerText;

const paramsSelector = document.querySelector('div[id=""bx_item-params""]');
const paramsTableSelector = paramsSelector?.querySelector('ul[class=""params__paramsList___XzY3MG""]');

const characteristics = paramsTableSelector
  ? Array.from(paramsTableSelector.querySelectorAll('li')).map(s => {
      const text = s.innerText;
      const parts = text.split(':');
      const name = parts[0]?.trim() || '';
      const value = parts[1]?.trim() || '';
      return { name, value };
    })
  : [];

const typeObj = characteristics.find(p => p.name === 'Вид запчасти');
const type = typeObj ? typeObj.value : null;
return { title, type };
}
";

        
        await Navigate(browserPage, spare.CatalogueRepresentation.Url);
        if (!await bypassFactory.Create(browserPage).Bypass())
        {
            throw new InvalidOperationException("Bypass failed.");
        }

        await browserPage.ScrollBottom();
        JsonData result = await browserPage.EvaluateFunctionAsync<JsonData>(javaScript);
        if (!result.AllPropertiesSet())
        {
            throw new InvalidOperationException("Not all properties set.");
        }

        AvitoSpareConcreteRepresentation representation = result.Representation();
        return spare.Concretized(representation);
    }

    private sealed class JsonData
    {
        public string? Title { get; set; }
        public string? Type { get; set; }

        public AvitoSpareConcreteRepresentation Representation() => new(Type!, Title!);

        public bool AllPropertiesSet() =>
            !string.IsNullOrWhiteSpace(Title) && !string.IsNullOrWhiteSpace(Type);
    }

    private static async Task Navigate(IPage page, string url)
    {
        try
        {
            await page.GoToAsync(url, _options);
        }
        catch (Exception)
        {
            
        }
    }
}
