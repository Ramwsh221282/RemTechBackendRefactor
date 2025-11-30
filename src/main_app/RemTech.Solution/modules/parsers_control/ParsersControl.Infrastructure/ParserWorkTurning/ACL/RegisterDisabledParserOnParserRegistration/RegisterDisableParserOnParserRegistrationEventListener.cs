using ParsersControl.Core.ParserRegistrationManagement;
using ParsersControl.Core.ParserRegistrationManagement.Contracts;
using ParsersControl.Core.ParserWorkStateManagement;
using ParsersControl.Core.ParserWorkStateManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserWorkTurning.ACL.RegisterDisabledParserOnParserRegistration;

public sealed class RegisterDisableParserOnParserRegistrationEventListener(
 IParserWorkTurnersStorage storage
 )
 : IOnParserRegisteredEventListener
{ 
 public async Task<Result<Unit>> React(ParserData data, CancellationToken ct = default)
 {
  ParserWorkTurnerState state = new ParserWorkTurnerState(data.Id, new ParserState.Disabled());
  ParserWorkTurner turner = new(state);
  await storage.Persist(turner, ct);
  return Unit.Value;
 }
}