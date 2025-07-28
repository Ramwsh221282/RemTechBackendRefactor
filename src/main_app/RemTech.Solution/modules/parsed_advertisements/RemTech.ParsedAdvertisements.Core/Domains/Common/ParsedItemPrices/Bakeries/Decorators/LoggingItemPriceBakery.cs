using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Receipts;
using RemTech.Result.Library;
using Serilog;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Decorators;

public sealed class LoggingItemPriceBakery(ILogger logger, IItemPriceBakery origin)
    : IItemPriceBakery
{
    public Status<IItemPrice> Baked(IItemPriceReceipt receipt)
    {
        logger.Information("Создание цены.");
        Status<IItemPrice> price = origin.Baked(receipt);
        if (price.IsSuccess)
        {
            logger.Information("Цена создана.");
            logger.Information("Значение цены: {0}.", (long)price.Value.Value());
            logger.Information("НДС имеется: {0}.", price.Value.UnderNds());
            return price;
        }
        logger.Error("Ошибка: {0}.", price.Error.ErrorText);
        return price;
    }
}
