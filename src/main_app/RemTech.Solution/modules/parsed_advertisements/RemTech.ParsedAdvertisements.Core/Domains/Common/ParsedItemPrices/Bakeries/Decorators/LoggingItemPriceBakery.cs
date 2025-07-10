using RemTech.Logging.Library;
using RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Receipts;
using RemTech.Result.Library;

namespace RemTech.ParsedAdvertisements.Core.Domains.Common.ParsedItemPrices.Bakeries.Decorators;

public sealed class LoggingItemPriceBakery(ICustomLogger logger, IItemPriceBakery origin)
    : IItemPriceBakery
{
    public Status<IItemPrice> Baked(IItemPriceReceipt receipt)
    {
        logger.Info("Создание цены.");
        Status<IItemPrice> price = origin.Baked(receipt);
        if (price.IsSuccess)
        {
            logger.Info("Цена создана.");
            logger.Info("Значение цены: {0}.", (long)price.Value.Value());
            logger.Info("НДС имеется: {0}.", price.Value.UnderNds());
            return price;
        }
        logger.Error("Ошибка: {0}.", price.Error.ErrorText);
        return price;
    }
}
