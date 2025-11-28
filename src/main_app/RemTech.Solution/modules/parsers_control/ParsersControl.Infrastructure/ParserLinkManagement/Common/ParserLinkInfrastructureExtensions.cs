using ParsersControl.Core.ParserLinksManagement;
using ParsersControl.Core.ParserLinksManagement.Contracts;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Infrastructure.ParserLinkManagement.Common;

public static class ParserLinkInfrastructureExtensions
{
    extension(ParserLinkData data)
    {
        public async Task<Result<Unit>> ParserAlreadyHasLink(Guid parserId, IParserLinksStorage storage, CancellationToken ct)
        {
            ParserLinkQueryArgs query = new(ParserId: parserId, Name: data.Name, Url: data.Url);
            ParserLink? link = await storage.Fetch(query, ct);
            return Error.Conflict("У парсера уже есть ссылка с таким URL и названием");
        }
    }
}