using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.ParserWorkStateManagement.Contracts;

public interface IParserWorkStatesStorage :
    IEntityPersister<ParserWorkTurner>,
    IEntityUpdater<ParserWorkTurner>,
    IEntityFetcher<ParserWorkTurner, ParserWorkTurnerQueryArgs>;