using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.ParserScheduleManagement.Contracts;

public sealed record ParserScheduleQueryArgs(Guid? Id = null, bool WithLock = false) : EntityFetchArgs;