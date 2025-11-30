using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.ParserStatisticsManagement.Contracts;

public interface IParserStatisticsStorage : 
    IEntityUpdater<ParserStatistic>, 
    IEntityPersister<ParserStatistic>,
    IEntityFetcher<ParserStatistic, ParserStatisticsQuery>;