using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.ParserStateManagement.Contracts;

public sealed record StatefulParserQueryArgs(Guid? Id = null, bool WithLock = false) 
    : EntityFetchArgs;

public interface IStatefulParsersStorage : IEntityFetcher<StatefulParser, StatefulParserQueryArgs>;