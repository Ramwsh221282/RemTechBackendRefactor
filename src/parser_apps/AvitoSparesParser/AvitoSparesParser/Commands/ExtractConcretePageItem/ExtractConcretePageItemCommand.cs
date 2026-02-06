using AvitoSparesParser.AvitoSpareContext;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace AvitoSparesParser.Commands.ExtractConcretePageItem;

public sealed class ResilientExtractConcretePageItemCommand : IExtractConcretePageItemCommand
{
    private readonly Serilog.ILogger _logger;
    private readonly IExtractConcretePageItemCommand _inner;
    private readonly int _attemptsCount;

    public ResilientExtractConcretePageItemCommand(
        Serilog.ILogger logger,
        IExtractConcretePageItemCommand inner,
        int attemptsCount = 5
    )
    {
        _logger = logger;
        _inner = inner;
        _attemptsCount = attemptsCount;
    }

    public async Task<AvitoSpare> Extract(AvitoSpare spare)
    {
        int currentAttempt = 0;
        while (currentAttempt < _attemptsCount)
        {
            try
            {
                return await _inner.Extract(spare);
            }
            catch (Exception ex)
            {
                if (currentAttempt == _attemptsCount)
                {
                    _logger.Error(
                        ex,
                        "Failed to extract concrete page item from url: {Url} after {Attempts} attempts.",
                        spare.CatalogueRepresentation.Url,
                        _attemptsCount
                    );
                    throw;
                }

                currentAttempt++;
                _logger.Warning(
                    ex,
                    "Attempt {Attempt} to extract concrete page item from url: {Url} failed. Retrying...",
                    currentAttempt,
                    spare.CatalogueRepresentation.Url
                );
            }
        }

        // в while true уже есть выход.
        throw new InvalidOperationException("Unreachable code.");
    }
}

public sealed class ExtractConcretePageItemCommand(
    Func<Task<IPage>> pageSource,
    AvitoBypassFactory bypassFactory
) : IExtractConcretePageItemCommand
{
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

        IPage page = await pageSource();
        await page.PerformQuickNavigation(spare.CatalogueRepresentation.Url, timeout: 2000);
        if (!await bypassFactory.Create(page).Bypass())
            throw new InvalidOperationException("Bypass failed.");

        JsonData result = await page.EvaluateFunctionAsync<JsonData>(javaScript);
        if (!result.AllPropertiesSet())
            throw new InvalidOperationException("Not all properties set.");

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
}
