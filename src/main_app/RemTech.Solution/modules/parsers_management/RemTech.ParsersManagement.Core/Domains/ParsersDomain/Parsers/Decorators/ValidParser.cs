using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkIdentities.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinksBags;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks.ValueObjects.ParserLinkUrls.Compares;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Errors;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;

public sealed class ValidParser(IParser inner) : IParser
{
    public ParserIdentity Identification() => inner.Identification();

    public ParserStatistic WorkedStatistics() => inner.WorkedStatistics();

    public ParserSchedule WorkSchedule() => inner.WorkSchedule();

    public ParserState WorkState() => inner.WorkState();

    public ParserLinksBag OwnedLinks() => inner.OwnedLinks();

    public ParserServiceDomain Domain() => inner.Domain();

    public Status<ParserStatisticsIncreasement> IncreaseProcessed(IParserLink link)
    {
        if (!WorkState().AtWork())
            return new IncreaseParserProcessedWhenNotWorking();
        if (!link.Activity())
            return new InactiveParserLinkProcessedIncreasementError(link);
        if (!new CompareLinkIdentityByParserOwning(link, inner))
            return new ParserLinkIsNotOfParserError(link, inner);
        return inner.IncreaseProcessed(link);
    }

    public Status ChangeState(NotEmptyString stateString) =>
        stateString switch
        {
            null => new StateEmptyError(),
            not null when WorkState().AtWork() => new EditParserWhenWorkingError(),
            not null when WorkState().IsState(stateString) == false =>
                new StringNotParserStateError(stateString),
            _ => inner.ChangeState(stateString),
        };

    public Status Enable()
    {
        ParserState current = WorkState();
        if (current.AtDisabled() == false)
            return new EnableNotDisabedError();
        return current.AtEnabled() ? new EnableAlreadyEnabledError() : inner.Enable();
    }

    public Status Disable()
    {
        bool disabled = WorkState().AtDisabled();
        return disabled ? new DisableDisabledParserError() : inner.Disable();
    }

    public Status ChangeWaitDays(PositiveInteger waitDays) =>
        waitDays switch
        {
            null => new WaitDaysEmptyError(),
            not null when WorkSchedule().WaitDaysTooMuch(waitDays) => new WaitDaysTooMuchError(
                waitDays
            ),
            not null when WorkSchedule().WaitDaysTooLess(waitDays) => new WaitDaysTooLessError(
                waitDays
            ),
            not null when WorkState().AtWork() => new EditParserWhenWorkingError(),
            _ => inner.ChangeWaitDays(waitDays),
        };

    public Status<IParserLink> Put(IParserLink link)
    {
        if (WorkState().AtWork())
            return new EditParserWhenWorkingError();
        if (!new CompareLinkIdentityByParserOwning(link, inner))
            return new ParserLinkIsNotOfParserError(link, inner);
        if (!new CompareLinkIdentityByParserServiceDomain(link, inner))
            return new ParserLinkIsNotOfParserServiceDomainError(link, inner);
        ParserLinksBag links = OwnedLinks();
        if (links.FindConcrete(l => new CompareLinkIdentityByName(l, link)).Any())
            return new ParserOwnsLinkWithNameError(inner, link);
        if (links.FindConcrete(l => new CompareParserLinkUrl(l, link)).Any())
            return new ParserOwnsLinkWithUrlError(inner, link);
        if (links.FindConcrete(l => new CompareLinkIdentityById(l, link)).Any())
            return new ParserOwnsLinkWithIdError(inner, link);
        return inner.Put(link);
    }

    public Status<IParserLink> Drop(IParserLink link)
    {
        if (WorkState().AtWork())
            return new EditParserWhenWorkingError();
        if (!new CompareLinkIdentityByParserOwning(link, inner))
            return new ParserLinkIsNotOfParserError(link, inner);
        if (!new CompareLinkIdentityByParserServiceDomain(link, inner))
            return new ParserLinkIsNotOfParserServiceDomainError(link, inner);
        return inner.Drop(link);
    }

    public Status<IParserLink> ChangeActivityOf(IParserLink link, bool nextActivity)
    {
        if (WorkState().AtWork())
            return new EditParserWhenWorkingError();
        if (!new CompareLinkIdentityByParserOwning(link, inner))
            return new ParserLinkIsNotOfParserError(link, inner);
        return inner.ChangeActivityOf(link, nextActivity);
    }

    public Status<IParserLink> Finish(IParserLink link, PositiveLong elapsed)
    {
        if (!WorkState().AtWork())
            return new FinishLinkWhenNotWorkingError(inner);
        if (!new CompareLinkIdentityByParserOwning(link, inner))
            return new ParserLinkIsNotOfParserError(link, inner);
        return inner.Finish(link, elapsed);
    }

    public Status Stop() => !WorkState().AtWork() ? new StopWhenNotWorkingError() : inner.Stop();

    public Status Start()
    {
        if (!WorkState().AtEnabled() || WorkState().AtWork())
            return new StartingWhenNotWaitingOrEnabledError();
        if (OwnedLinks().Amount().Same(PositiveInteger.New()))
            return new StartingWhenHasNoLinksError(inner);
        if (new InactiveParserLinksBag(OwnedLinks()).Any())
            return new StartingWhenHasNoActiveLinksError(inner);
        return inner.Start();
    }
}
