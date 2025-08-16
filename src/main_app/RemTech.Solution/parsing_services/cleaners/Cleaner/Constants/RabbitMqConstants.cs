namespace Cleaner.Constants;

internal static class RabbitMqConstants
{
    public const string CleanersExchange = "cleaners";
    public const string CleanersStartQueue = "start";
    public const string CleanersCleanItemQueue = "cleanitem";
    public const string CleanersFinishQueue = "finish";
}
