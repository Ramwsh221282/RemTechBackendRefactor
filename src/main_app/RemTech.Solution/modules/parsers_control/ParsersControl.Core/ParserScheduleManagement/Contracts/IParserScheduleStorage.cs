using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.ParserScheduleManagement.Contracts;

public interface IParserScheduleStorage :
    IEntityFetcher<ParserSchedule, ParserScheduleQueryArgs>,
    IEntityPersister<ParserSchedule>,
    IEntityUpdater<ParserSchedule>;