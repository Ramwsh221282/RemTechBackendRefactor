using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using RemTech.Result.Pattern;
using Telemetry.Contracts;

namespace Telemetry.Gateways.Filters;

internal static class SaveActionEventExtensions
{
    internal static SaveActionInfoEvent AddElapsedTime(
        this SaveActionInfoEvent @event,
        Stopwatch stopwatch
    )
    {
        stopwatch.Stop();
        long elapsed = stopwatch.ElapsedMilliseconds;
        string comment = $"Действие выполнено за: {elapsed} секунд.";
        IEnumerable<string> comments = [comment, .. @event.Comments];
        return @event with { Comments = comments };
    }
}

public sealed class TestFilter : Attribute, IAsyncActionFilter
{
    private const string Token = "RemTechAccessTokenId";
    private readonly Serilog.ILogger _logger;
    private readonly string _actionName;

    public TestFilter(Serilog.ILogger logger, string actionName)
    {
        _logger = logger;
        _actionName = actionName;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        await next();
        Result<Guid> tokenResult = ExtractTokenId(context);
        SaveActionInfoEvent @event = CreateDefaultEvent();
        @event = @event.AddElapsedTime(stopwatch);
    }

    private SaveActionInfoEvent CreateDefaultEvent()
    {
        DateTime ocucred = DateTime.UtcNow;
        return new SaveActionInfoEvent([], _actionName, string.Empty, Guid.Empty, ocucred);
    }

    private SaveActionInfoEvent AddElapsedTimeComment(
        SaveActionInfoEvent @event,
        Stopwatch stopwatch
    )
    {
        stopwatch.Stop();
        long elapsed = stopwatch.ElapsedMilliseconds;
        string comment = $"Действие выполнено за: {elapsed} секунд.";
        IEnumerable<string> comments = [comment, .. @event.Comments];
        return @event with { Comments = comments };
    }

    private Result<Guid> ExtractTokenId(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(Token, out StringValues tokenString))
            return Error.NotFound("Не найден токен.");
        if (!Guid.TryParse(tokenString, out Guid tokenId))
            return Error.Validation("Токен некорректный.");
        return tokenId;
    }

    // private static bool IsAccessTokenGuid(StringValues accessTokenIdValue, out Guid guidToken)
    // {
    //     bool isGuid = Guid.TryParse(accessTokenIdValue, out Guid tokenGuid);
    //     guidToken = tokenGuid;
    //     return isGuid;
    // }
    //
    // private static bool HasAccessTokenId(HttpContext context, out StringValues accessTokenIdValue)
    // {
    //     bool has = context.Request.Headers.TryGetValue(
    //         "RemTechAccessTokenId",
    //         out StringValues accessTokenId
    //     );
    //     accessTokenIdValue = accessTokenId;
    //     return has;
    // }
}
