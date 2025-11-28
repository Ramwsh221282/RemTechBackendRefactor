using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.ParserLinksManagement.Contracts;

public sealed record ParserLinkQueryArgs(
    Guid? Id = null, 
    string? Name = null, 
    string? Url = null,
    Guid? ParserId = null,
    bool WithLock = false) : EntityFetchArgs;