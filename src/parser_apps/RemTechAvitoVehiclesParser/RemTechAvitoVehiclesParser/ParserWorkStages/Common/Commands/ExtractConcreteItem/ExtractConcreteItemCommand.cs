using AvitoFirewallBypass;
using ParsingSDK.Parsing;
using PuppeteerSharp;

namespace RemTechAvitoVehiclesParser.ParserWorkStages.Common.Commands.ExtractConcreteItem;

public sealed class ExtractConcreteItemCommand(
    IPage page,
    AvitoVehicle vehicle,
    AvitoBypassFactory bypassFactory
) : IExtractConcreteItemCommand
{
    private static readonly Dictionary<string, bool> IgnoredCharacteristics = new()
    {
        ["Марка"] = true,
        ["Модель"] = true,
        ["Тип техники"] = true,
        ["ПТС"] = true,
        ["ПСМ"] = true,
        ["Состояние"] = true,
        ["VIN"] = true,
        ["Доступность"] = true,
    };

    public async Task<AvitoVehicle> Handle()
    {
        const string javaScript =
            @"
                                  () => { 
                                  const breadCrumbsSelector = document.querySelector('div[id=""bx_item-breadcrumbs""]');
                                  const breadCrumbs = Array.from(breadCrumbsSelector.querySelectorAll('span[itemprop=""itemListElement""]'))
                                  .map(s => {
                                      const breadCrumbText = s.querySelector('span[itemprop=""name""]');
                                      if (!breadCrumbText) return '';
                                      return breadCrumbText.innerText;
                                    });
                                  const category = breadCrumbs[breadCrumbs.length-3];
                                  const brand = breadCrumbs[breadCrumbs.length-2];
                                  const model = breadCrumbs[breadCrumbs.length-1];
                                  const title = category + ' ' + brand + ' ' + model

                                  const paramsSelector = document.querySelector('div[id=""bx_item-params""]');
                                  const paramsTableSelector = paramsSelector.querySelector('ul[class=""params__paramsList___XzY3MG""]');
                                  const characteristics = Array.from(paramsTableSelector.querySelectorAll('li')).map(s => {
                                      const text = s.innerText;
                                      const parts = text.split(':');
                                      const name = parts[0].trim();
                                      const value = parts[1].trim();
                                      return { name: name, value: value }
                                  });
                                  return { title: title, characteristics: characteristics };
                                  }
                                  ";
        
        await page.PerformQuickNavigation(vehicle.CatalogueRepresentation.Url, timeout: 1500);
        if (!await bypassFactory.Create(page).Bypass())
            throw new InvalidOperationException("Unable to bypass Avito firewall");

        await page.ResilientWaitForSelector("div[id=\"bx_item-params\"]");
        JsonExtractConcreteItemCommandData json =
            await page.EvaluateFunctionAsync<JsonExtractConcreteItemCommandData>(javaScript);
        if (!json.AllPropertiesSet())
            throw new InvalidOperationException("Unable to extract concrete item data");

        json.Characteristics = [.. json.Characteristics!.Where(NotBelongsToIgnoredCharacteristics)];
        AvitoVehicleConcretePageRepresentation representation = new(
            json.Title!,
            json.CharacteristicsDictionary()
        );

        return vehicle.CopyWith(veh =>
            new()
            {
                ConcretePageRepresentation = representation,
                CatalogueRepresentation = veh.CatalogueRepresentation,
                Processed = false,
                RetryCount = 0,
            }
        );
    }

    private static bool NotBelongsToIgnoredCharacteristics(JsonCharacteristic c)
    {
        string name = c.Name;
        bool belongs = IgnoredCharacteristics.ContainsKey(name);
        return !belongs;
    }

    private sealed class JsonExtractConcreteItemCommandData
    {
        public string? Title { get; set; }
        public JsonCharacteristic[]? Characteristics { get; set; }

        public bool AllPropertiesSet() => Title != null && Characteristics != null;

        public Dictionary<string, string> CharacteristicsDictionary()
        {
            Dictionary<string, string> result = [];
            foreach (JsonCharacteristic characteristic in Characteristics!)
                result.Add(characteristic.Name, characteristic.Value);
            return result;
        }
    }

    private sealed class JsonCharacteristic
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
