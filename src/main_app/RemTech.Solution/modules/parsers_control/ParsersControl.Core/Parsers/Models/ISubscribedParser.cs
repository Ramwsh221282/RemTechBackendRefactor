using ParsersControl.Core.ParserLinks.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.Parsers.Models;

public interface ISubscribedParser
{
    Result<SubscribedParserLink> AddLink(SubscribedParserLinkUrlInfo urlInfo);
    Result<SubscribedParser> Enable();
    Result<SubscribedParser> AddParserAmount(int amount);
    Result<SubscribedParser> AddWorkTime(long totalElapsedSeconds);
    SubscribedParser ResetWorkTime();
    SubscribedParser ResetParsedCount();
    Result<SubscribedParser> StartWork();
    SubscribedParser Disable();
    Result<SubscribedParser> FinishWork();
    Result<SubscribedParser> ChangeScheduleWaitDays(int waitDays);
    Result<SubscribedParser> ChangeScheduleNextRun(DateTime nextRun);
}