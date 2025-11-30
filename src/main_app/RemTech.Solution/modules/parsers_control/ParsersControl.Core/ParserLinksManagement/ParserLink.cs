using ParsersControl.Core.ParserLinksManagement.Contracts;
using ParsersControl.Core.ParserLinksManagement.Defaults;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;

namespace ParsersControl.Core.ParserLinksManagement;

public sealed class ParserLink(ParserLinkData data)
{
    private readonly ParserLinkData _data = data;
    private readonly IParserLinkRenamedListener _onRenamed = new NoneParserLinkRenamedListener();
    private readonly IParserLinkIgnoredListener _onIgnored = new NoneParserLinkIgnoredListener();
    private readonly IParserLinkParserAttached _onAttached = new NoneParserLinkAttachedListener();
    private readonly IParserLinkUnignoredListener _onUnignored = new NoneParserLinkUnignoredListener();
    private readonly IParserLinkUrlChangedListener _onUrlChanged = new NoneParserLinkUrlChangedListener();

    public async Task<Result<ParserLink>> Rename(string newName, CancellationToken ct)
    {
        ParserLink updated = Copy(this, name: newName);
        Result<Unit> validation = updated.ValidateState();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> reacting = await _onRenamed.React(updated._data, ct);
        return reacting.IsFailure ? reacting.Error : updated;
    }

    public async Task<Result<ParserLink>> ChangeUrl(string newUrl, CancellationToken ct)
    {
        ParserLink updated = Copy(this, url: newUrl);
        Result<Unit> validation = updated.ValidateState();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> reacting = await _onUrlChanged.React(updated._data, ct);
        return reacting.IsFailure ? reacting.Error : updated;
    }

    public async Task<Result<ParserLink>> MakeIgnored(CancellationToken ct)
    {
        ParserLink updated = Copy(this, ignorance: true);
        Result<Unit> validation = updated.ValidateState();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> reacting = await _onIgnored.React(updated._data, ct);
        return reacting.IsFailure ? reacting.Error : updated;
    }

    public async Task<Result<ParserLink>> Unignore(CancellationToken ct)
    {
        ParserLink updated = Copy(this, ignorance: false);
        Result<Unit> validation = updated.ValidateState();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> reacting = await _onUnignored.React(updated._data, ct);
        return reacting.IsFailure ? reacting.Error : updated;
    }

    public async Task<Result<ParserLink>> AttachToParser(Guid id, CancellationToken ct)
    {
        ParserLink attached = new ParserLink(_data with { ParserId = id });
        ParserLink copied = attached.Copy(attached);
        Result<Unit> validation = copied.ValidateState();
        if (validation.IsFailure) return validation.Error;
        Result<Unit> attaching = await _onAttached.React(id, copied._data, ct);
        return attaching.IsFailure ? attaching.Error : this;
    }

    public ParserLink AddListener(IParserLinkRenamedListener listener) => new(this, onRenamed: listener);
    public ParserLink AddListener(IParserLinkUrlChangedListener listener) => new(this, onUrlChanged: listener);
    public ParserLink AddListener(IParserLinkIgnoredListener listener) => new(this, onIgnored: listener);
    public ParserLink AddListener(IParserLinkUnignoredListener listener) => new(this, onUnignored: listener);
    public ParserLink AddListener(IParserLinkParserAttached listener) => new(this, onAttached: listener);

    private ParserLink(ParserLink origin, 
        IParserLinkRenamedListener? onRenamed = null,
        IParserLinkUrlChangedListener? onUrlChanged = null,
        IParserLinkIgnoredListener? onIgnored = null,
        IParserLinkUnignoredListener? onUnignored = null,
        IParserLinkParserAttached? onAttached = null) 
        : this(origin._data)
    {
        _onRenamed = onRenamed ?? origin._onRenamed;
        _onUrlChanged = onUrlChanged ?? origin._onUrlChanged;
        _onIgnored = onIgnored ?? origin._onIgnored;
        _onUnignored = onUnignored ?? origin._onUnignored;
        _onAttached = onAttached ?? origin._onAttached;
    }

    public void Write(
        Action<Guid>? writeId = null,
        Action<string>? writeUrl = null,
        Action<string>? writeName = null,
        Action<bool>? writeIgnorance = null,
        Action<Guid>? writeParserId = null)
    {
        writeId?.Invoke(_data.Id);
        writeUrl?.Invoke(_data.Url);
        writeName?.Invoke(_data.Name);
        writeIgnorance?.Invoke(_data.Ignored);
        writeParserId?.Invoke(_data.ParserId);
    }
    
    private ParserLink Copy(ParserLink origin, string? name = null, string? url = null, bool? ignorance = null)
    {
        ParserLinkData changingData = origin._data;
        if (name != null) changingData = changingData with { Name = name };
        if (url != null) changingData = changingData with { Url = url };
        if (ignorance != null) changingData = changingData with { Ignored = ignorance.Value };
        return new ParserLink(changingData)
            .AddListener(_onRenamed)
            .AddListener(_onIgnored)
            .AddListener(_onAttached)
            .AddListener(_onUnignored)
            .AddListener(_onUrlChanged);
    }
    
    private Result<Unit> ValidateState()
    {
        List<string> errors = [];
        const string urlName = "URL ссылки парсера";
        const string nameName = "название ссылки парсера";
        const string idName = "идентификатор ссылки парсера";
        const string parserIdName = "идентификатор парсера";
        const int maxNameLength = 256;
        if (_data.Id == Guid.Empty) errors.Add(Error.NotSet(idName));
        if (string.IsNullOrWhiteSpace(_data.Url)) errors.Add(Error.NotSet(urlName));
        if (string.IsNullOrWhiteSpace(_data.Name)) errors.Add(Error.NotSet(nameName));
        if (_data.Name.Length > maxNameLength) errors.Add(Error.GreaterThan(nameName, maxNameLength));
        if (_data.ParserId == Guid.Empty) errors.Add(Error.NotSet(parserIdName));
        return errors.Count == 0 ? Unit.Value : Error.Validation(errors);
    }
}