using RemTech.Core.Shared.Primitives;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.Decorators;

public sealed class ParserIdBag : IMaybeParserId
{
    private MaybeBag<NotEmptyGuid> _bag;

    public ParserIdBag() => _bag = new MaybeBag<NotEmptyGuid>();

    public ParserIdBag(NotEmptyGuid id) => _bag = new MaybeBag<NotEmptyGuid>(id);

    public ParserIdBag(Guid id)
        : this(NotEmptyGuid.New(id)) { }

    public ParserIdBag(Guid? id)
        : this(NotEmptyGuid.New(id)) { }

    public ParserIdBag(Status<NotEmptyGuid> guid) =>
        _bag = guid.IsFailure ? new MaybeBag<NotEmptyGuid>() : new MaybeBag<NotEmptyGuid>(guid);

    public NotEmptyGuid Take() => _bag.Take();

    public void Put(NotEmptyGuid parserId)
    {
        if (_bag.Any())
            return;
        _bag = _bag.Put(parserId);
    }

    public static implicit operator ParserIdBag(Guid id) => new(id);

    public static implicit operator ParserIdBag(Guid? id) => new(id);

    public static implicit operator ParserIdBag(NotEmptyGuid id) => new(id);

    public static implicit operator ParserIdBag(Status<NotEmptyGuid> id) => new(id);
}
