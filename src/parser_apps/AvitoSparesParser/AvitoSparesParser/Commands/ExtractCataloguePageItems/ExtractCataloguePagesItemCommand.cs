using System.Text.RegularExpressions;
using AvitoSparesParser.AvitoSpareContext;
using AvitoSparesParser.CatalogueParsing;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace AvitoSparesParser.Commands.ExtractCataloguePageItems;

public sealed partial class ExtractCataloguePagesItemCommand(
    IPage browserPage,
    AvitoBypassFactory bypassFactory
) : IExtractCataloguePagesItemCommand
{    
    private static readonly NavigationOptions _options = new()
    {
        Timeout = 3000,
        WaitUntil = [WaitUntilNavigation.Load]
    };

    public async Task<AvitoSpare[]> Extract(AvitoCataloguePage page)
    {
        const string javaScript =
            @"() => {                    
                                const photoExtractFn = (item) => {
                                    const photoListSelector = item.querySelector('div[data-marker=""item-image""]') 
                                    ?.querySelector('div[data-marker=""item-photo""]')
                                    ?.querySelector('ul');
                                    if (!photoListSelector) return [];
                                    return Array.from(photoListSelector.querySelectorAll('li')).map(s => {
                                        const photo = s.querySelector('img');
                                        if (!photo) return '';
                                        const srcSet = photo.getAttribute('srcset');
                                        if (!srcSet) return '';
                                        const splittedParts = srcSet.split(',');
                                        return splittedParts[splittedParts.length-1].split(' ')[0];
                                    });
                                };

                                const itemSelectors = Array.from(document.querySelectorAll('div[data-marker=""item""]'));
                                const data = itemSelectors.map((i) => {
                                      // url extraction    
                                      const urlValue = ""https://avito.ru"" + i.querySelector('h2[itemprop=""name""]')
                                                                                .querySelector('a[itemprop=""url""]')
                                                                                .getAttribute(""href"");
                                      // price and is nds extraction
                                      const priceSelector = i.querySelector('p[data-marker=""item-price""]');
                                      const priceValue = priceSelector.querySelector('meta[itemprop=""price""]').getAttribute(""content"");
                                      const isNds = priceSelector.innerText.includes(""НДС"");
                                      // id
                                      const idValue = ""avito_spare_"" + i.getAttribute(""data-item-id"");
                                      // address
                                      const address = i.querySelector('div[data-marker=""item-location""]').querySelector('span[title]').innerText;
                                      // photos
                                      const photos = photoExtractFn(i);     
                                      // oem extraction
                                      const oemValue = i.querySelector('p[data-marker=""item-oem-number""]')?.innerText;
                                                                
                                      return { url: urlValue, price: priceValue, isNds: isNds, id: idValue, address: address, photos: photos, oem: oemValue }
                                  });
                                  return data; 
                            }
";

        string url = page.Url;
        await Navigate(browserPage, url);
        if (!await bypassFactory.Create(browserPage).Bypass())
        {
            throw new InvalidOperationException("Bypass failed.");
        }

        await browserPage.ScrollBottom();
        await new AvitoImagesHoverer(browserPage).Invoke();

        JsonData[] data = await browserPage.EvaluateFunctionAsync<JsonData[]>(javaScript);
        return
        [
            .. data.Where(d => d.AllPropertiesSet() && SatisfiesOemFilter(d))
                .Select(d => d.ToAvitoSpareCatalogueRepresentation()),
        ];
    }

    private static bool SatisfiesOemFilter(JsonData data)
    {
        if (string.IsNullOrWhiteSpace(data.Oem))
        {
            return false;
        }

        if (!data.Oem.Any(char.IsDigit))
        {
            return false;
        }

        if (data.Oem.Trim() == "0")
        {
            return false;
        }

        return OEM_FILTER_REGEX().IsMatch(data.Oem);
    }

    private sealed class JsonData
    {
        public string? Url { get; set; }
        public string? Price { get; set; }
        public bool IsNds { get; set; }
        public string? Id { get; set; }
        public string? Address { get; set; }
        public string[]? Photos { get; set; }
        public string? Oem { get; set; }

        public AvitoSpare ToAvitoSpareCatalogueRepresentation()
        {
            var representation = new AvitoSpareCatalogueRepresentation(
                Url!,
                long.Parse(Price!),
                IsNds,
                Address!,
                Photos!,
                Oem!
            );
            return AvitoSpare.CatalogueRepresented(Id!, representation);
        }

        public bool AllPropertiesSet()
        {
            return !string.IsNullOrWhiteSpace(Url)
                && PriceIsNotZero()
                && !string.IsNullOrWhiteSpace(Id)
                && !string.IsNullOrWhiteSpace(Address)
                && !string.IsNullOrWhiteSpace(Oem)
                && Photos != null
                && Photos.Length > 0;
        }

        private bool PriceIsNotZero()
        {
            return !string.IsNullOrWhiteSpace(Price) && Price != "0";
        }
    }

    private static async Task Navigate(IPage page, string url)
    {
        try
        {
            await page.GoToAsync(url, _options);
        }
        catch
        {
            
        }
    }

    // фильтрует OEM номера от лишних OEM.
    // То бишь, если нет как минимум 4 символа цифры подряд, то такой OEM не рассматривается.
    // Потому что могут быть просто какие-то рандомные названия, а не артикулы.
    [GeneratedRegex(@"(\d+){4,}")]
    private static partial Regex OEM_FILTER_REGEX();
}
