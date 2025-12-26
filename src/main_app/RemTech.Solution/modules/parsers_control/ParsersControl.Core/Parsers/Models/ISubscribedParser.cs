using ParsersControl.Core.ParserLinks.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public interface ISubscribedParser
{
    Result<SubscribedParserLink> RemoveLink(SubscribedParserLink link);
    Result<SubscribedParserLink> AddLinkParsedAmount(SubscribedParserLink link, int count);
    Result<SubscribedParserLink> AddLinkWorkTime(SubscribedParserLink link, long totalElapsedSeconds);
    Result<SubscribedParserLink> EditLink(SubscribedParserLink link, string? newName, string? newUrl);
    Result<SubscribedParser> Enable();
    Result<IEnumerable<SubscribedParserLink>> AddLinks(IEnumerable<SubscribedParserLinkUrlInfo> urlInfos);
    Result<SubscribedParser> AddParserAmount(int amount);
    Result<SubscribedParser> AddWorkTime(long totalElapsedSeconds);
    SubscribedParser ResetWorkTime();
    SubscribedParser ResetParsedCount();
    Result<SubscribedParser> StartWork();
    SubscribedParser Disable();
    Result<SubscribedParser> FinishWork();
    Result<SubscribedParser> ChangeScheduleWaitDays(int waitDays);
    Result<SubscribedParser> ChangeScheduleNextRun(DateTime nextRun);
    Result<SubscribedParserLink> FindLink(Func<SubscribedParserLinkUrlInfo, bool> predicate);
    Result<SubscribedParserLink> FindLink(Guid id);
    Result<SubscribedParserLink> FindLink(SubscribedParserLinkId id);
    Result<SubscribedParserLink> ChangeLinkActivity(SubscribedParserLink link, bool isActive);
}