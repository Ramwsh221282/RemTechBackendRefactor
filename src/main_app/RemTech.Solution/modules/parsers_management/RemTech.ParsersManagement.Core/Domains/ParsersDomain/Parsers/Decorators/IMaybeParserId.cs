using RemTech.ParsersManagement.Core.Common.Primitives;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;

public interface IMaybeParserId
{
    public NotEmptyGuid Take();
    public void Put(NotEmptyGuid parserId);
}
