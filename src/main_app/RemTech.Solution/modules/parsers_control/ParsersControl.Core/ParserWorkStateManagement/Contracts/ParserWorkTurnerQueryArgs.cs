using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.ParserWorkStateManagement.Contracts;

public sealed record ParserWorkTurnerQueryArgs(Guid? Id = null, bool WithLock = false) : EntityFetchArgs;