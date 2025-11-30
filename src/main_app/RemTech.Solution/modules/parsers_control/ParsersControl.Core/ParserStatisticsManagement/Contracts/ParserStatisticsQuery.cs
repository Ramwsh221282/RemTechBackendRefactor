using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.ParserStatisticsManagement.Contracts;

public sealed record ParserStatisticsQuery(Guid? Id = null, bool WithLock = false) : EntityFetchArgs;