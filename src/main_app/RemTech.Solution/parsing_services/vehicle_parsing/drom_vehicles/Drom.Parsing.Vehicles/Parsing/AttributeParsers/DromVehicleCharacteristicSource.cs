using Drom.Parsing.Vehicles.Parsing.Models;
using Parsing.SDK.ElementSources;
using Parsing.SDK.ScrapingArtifacts;
using PuppeteerSharp;

namespace Drom.Parsing.Vehicles.Parsing.AttributeParsers;

public sealed class DromVehicleCharacteristicSource(IPage page)
{
    private readonly string _containerSelector = string.Intern(".css-1bwl6o2.epjhnwz0");
    private readonly string _innerContainerSelector = string.Intern(".css-0.epjhnwz1");

    public async Task<CarCharacteristicsCollection> Read()
    {
        IElementHandle container = await new ValidSingleElementSource(
            new PageElementSource(page)
        ).Read(_containerSelector);
        IElementHandle innerContainer = await new ValidSingleElementSource(
            new ParentElementSource(container)
        ).Read(_innerContainerSelector);
        IElementHandle table = await new ValidSingleElementSource(
            new ParentElementSource(innerContainer)
        ).Read(string.Intern("table"));
        IElementHandle[] tableRows = await new ParentManyElementsSource(table).Read(
            string.Intern("tr")
        );
        CarCharacteristicsCollection collection = new();
        foreach (IElementHandle row in tableRows)
        {
            try
            {
                IElementHandle ctxName = await new ValidSingleElementSource(
                    new ParentElementSource(row)
                ).Read("th");
                IElementHandle ctxValue = await new ValidSingleElementSource(
                    new ParentElementSource(row)
                ).Read("td");
                string name = await new TextFromWebElement(ctxName).Read();
                if (string.IsNullOrWhiteSpace(name))
                    continue;
                if (name.Contains("Ходовая часть"))
                    continue;
                if (name.Contains("Тип техники"))
                    continue;
                string value = await new TextFromWebElement(ctxValue).Read();
                if (string.IsNullOrWhiteSpace(value))
                    continue;
                if (name == "Моточасы")
                    value = new string(value.Where(char.IsDigit).ToArray());
                if (name == "Масса")
                {
                    value = new string(value.Where(char.IsDigit).ToArray());
                    name = "Эксплуатационная масса";
                }

                if (name == "Год выпуска")
                    value = new string(value.Where(char.IsDigit).ToArray());
                if (name == "Грузоподъемность")
                {
                    value = new string(value.Where(char.IsDigit).ToArray());
                    name = "Грузоподъёмность";
                }
                if (name == "Мощность")
                    name = "Мощность двигателя";
                if (name == "Двигатель")
                    name = "Объём двигателя";
                if (value.Contains(", без пробега по РФ"))
                    value = value.Replace(", без пробега по РФ", string.Empty);
                if (value.Contains('?'))
                    value = value.Replace("?", string.Empty);
                if (value.Contains("без пробега по РФ"))
                    value = value.Replace("без пробега по РФ", string.Empty);
                if (string.IsNullOrWhiteSpace(value))
                    continue;
                collection.With(name, value);
            }
            catch
            {
                // ignored
            }
        }

        return collection;
    }
}
