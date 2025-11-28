using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.ParserRegistrationManagement.Contracts;

public sealed record RegisteredParserQuery(
    Guid? Id = null, 
    string? Domain = null, 
    string? Type = null, 
    bool WithLock = false) : EntityFetchArgs;

public interface IRegisteredParsersStorage :
    IEntityPersister<RegisteredParser>,
    IEntityFetcher<RegisteredParser, RegisteredParserQuery>;