using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using ParsersControl.Core.ParserWorkStateManagement;
using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using ParsersControl.Infrastructure.ParserWorkStateManagement.Common;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserWorkStateManagement.ACL.RegisterDisabledParserOnParserRegistration;

public sealed class RegisterDisableParserOnParserRegistrationEventListener(
    IParserWorkStatesStorage storage
)
    : IOnParserRegisteredEventListener
{
    public async Task<Result<Unit>> React(ParserData data, CancellationToken ct = default)
    {
        ParserWorkTurnerState state = new ParserWorkTurnerState(data.Id, new ParserState.Disabled());
        Result<Unit> validation = await storage.ValidateUniquesness(state, ct);
        if (validation.IsFailure) return validation.Error;
        ParserWorkTurner turner = new(state);
        await storage.Persist(turner, ct);
        return Unit.Value;
    }
}