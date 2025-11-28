using ParsersControl.Core.ParserLinksManagement;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Presenters.ParserLinkManagement.Common;

public delegate Task<Result<ParserLink>> FetchParserLinkById(Guid id, CancellationToken ct); 