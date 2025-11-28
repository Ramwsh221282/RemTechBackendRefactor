using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.ParserLinksManagement.Contracts;

public interface IParserLinksStorage : 
    IEntityPersister<ParserLink>, 
    IEntityFetcher<ParserLink, ParserLinkQueryArgs>,
    IEntityUpdater<ParserLink>;