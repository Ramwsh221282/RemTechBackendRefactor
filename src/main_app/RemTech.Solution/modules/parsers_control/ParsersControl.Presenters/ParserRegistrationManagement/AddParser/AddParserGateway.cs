using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Infrastructure.ParserRegistrationManagement.EventListeners;
using ParsersControl.Infrastructure.ParserScheduleManagement.Listeners.OnParserCreated;
using ParsersControl.Infrastructure.ParserStatistics.EventListeners.OnParserRegistered;
using ParsersControl.Infrastructure.ParserWorkTurning.ACL.RegisterDisabledParserOnParserRegistration;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserRegistrationManagement.AddParser;

public sealed class AddParserGateway(
    RegisterDisableParserOnParserRegistrationEventListener stateListener,
    LoggingOnParserRegisteredEventListener loggingListener,
    NpgSqlOnParserRegisteredEventListener npgSqlListener,
    RegisterEmptyStatisticsOnParserRegisteredListener statisticsListener,
    AddScheduleForParserOnParserCreatedEventListener scheduleListener
) 
    : IGateway<AddParserRequest, AddParserResponse>
{
    public async Task<Result<AddParserResponse>> Execute(AddParserRequest request)
    {
        AddParserResponse response = new();
        ParserRegistrationOffice office = new(new ParserData(Guid.NewGuid(), request.Type, request.Domain));
        
        ParserRegistrationOffice observed = office.AddListener(loggingListener
            .Wrap(npgSqlListener)
            .Wrap(stateListener)
            .Wrap(statisticsListener)
            .Wrap(scheduleListener)
            .Wrap(response));

        Result<Unit> registration = await observed.Register(request.Ct);
        return registration.IsFailure ? registration.Error : response;
    }
}