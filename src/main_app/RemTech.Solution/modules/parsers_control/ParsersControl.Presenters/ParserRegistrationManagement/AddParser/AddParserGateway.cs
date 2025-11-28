using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Infrastructure.ParserRegistrationManagement.EventListeners;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserRegistrationManagement.AddParser;

public sealed class AddParserGateway(
    LoggingOnParserRegisteredEventListener loggingListener,
    NpgSqlOnParserRegisteredEventListener npgSqlListener
) 
    : IGateway<AddParserRequest, AddParserResponse>
{
    public async Task<Result<AddParserResponse>> Execute(AddParserRequest request)
    {
        AddParserResponse response = new();
        ParserRegistrationOffice office = new(new ParserData(Guid.NewGuid(), request.Type, request.Domain));
        ParserRegistrationOffice observed = office.AddListener(loggingListener
            .Wrap(npgSqlListener)
            .Wrap(response));

        Result<Unit> registration = await observed.Register(request.Ct);
        return registration.IsFailure ? registration.Error : response;
    }
}