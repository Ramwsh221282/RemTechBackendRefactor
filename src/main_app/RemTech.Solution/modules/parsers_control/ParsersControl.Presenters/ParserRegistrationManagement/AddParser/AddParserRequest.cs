using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserRegistrationManagement.AddParser;

public record AddParserRequest(string Domain, string Type, CancellationToken Ct) : IRequest;