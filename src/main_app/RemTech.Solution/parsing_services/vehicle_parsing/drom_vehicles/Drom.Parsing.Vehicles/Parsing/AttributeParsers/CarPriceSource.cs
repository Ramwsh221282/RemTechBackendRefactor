using Drom.Parsing.Vehicles.Parsing.Models;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.AttributeParsers;

public sealed class CarPriceSource(IPage page)
{
    private readonly string _container = string.Intern(".css-pmcte9.e1ab30xp0");
    private readonly string _ndsContainer = string.Intern(".css-17b8egf.ez4q5ut1");

    public async Task<CarPrice> Read()
    {
        IElementHandle element = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(_container);
        string text = await new TextFromWebElement(element).Read();
        long priceValue = long.Parse(new string(text.Where(char.IsDigit).ToArray()));
        try
        {
            IElementHandle ndsElement = await new ValidSingleElementSource(
                new ParentElementSource(element)
            ).Read(_ndsContainer);
            await new TextFromWebElement(ndsElement).Read();
            return new CarPrice(priceValue, true);
        }
        catch
        {
            return new CarPrice(priceValue, false);
        }
    }
}
