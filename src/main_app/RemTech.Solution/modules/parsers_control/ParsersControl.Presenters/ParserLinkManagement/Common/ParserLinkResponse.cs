using ParsersControl.Core.ParserLinksManagement;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Presenters.ParserLinkManagement.Common;

public sealed class ParserLinkResponse : IResponse
{
    public Guid Id { get; private set; } = Guid.Empty;
    
    private void AddId(Guid id) => Id = id;
    
    public ParserLinkResponse(ParserLink link)
    {
        link.Write(AddId);
    }
}