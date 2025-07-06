using RemTech.Logging.Library;
using RemTech.ParsersManagement.Core.Common.Primitives;
using RemTech.ParsersManagement.Core.Common.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Async.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingNewParser.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Async.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.AddingParserLink.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Async;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Async.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.ChangingLinkActivity.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Async.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.DisablingParser.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Async.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.EnablingParser.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Async;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Async.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.FinishingParserLink.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Async;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Async.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.IncreasingProcessed.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Async;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Async.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.RemovingParserLink.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Async;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Async.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StartingParser.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Async;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Async.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.StoppedParser.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Async.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Features.UpdatingParser.Decorators;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.ParserLinks;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Parsers.ValueObjects;
using RemTech.ParsersManagement.Core.Domains.ParsersDomain.Ports;
using RemTech.ParsersManagement.Tests.Library.Asserts;
using RemTech.ParsersManagement.Tests.Library.Mocks.CoreLogic;
using RemTech.Result.Library;

namespace RemTech.ParsersManagement.Tests.Library;

public sealed class ParserTestingToolkit
{
    private readonly ICustomLogger _logger;
    private readonly ParsersSource _parsers;

    public ParserTestingToolkit(ParsersFixture fixture)
    {
        _logger = fixture.AccessLogger();
        _parsers = fixture.AccessParsersSource();
    }

    public ParserTestingToolkit(ICustomLogger logger, ParsersSource source)
    {
        _logger = logger;
        _parsers = source;
    }

    public async Task<Status<IParser>> ReadFromDataSource(string parserName)
    {
        Status<IParser> parser = await new AsyncAssertStatusSuccess<IParser>(
            () => _parsers.Find(new Name(NotEmptyString.New(parserName)))
        ).AsyncAsserted();
        return parser;
    }

    public async Task<Status<IParser>> ReadFromDataSource(Guid parserId)
    {
        Status<IParser> parser = await new AsyncAssertStatusSuccess<IParser>(
            () => _parsers.Find(NotEmptyGuid.New(parserId))
        ).AsyncAsserted();
        return parser;
    }

    public void AddNewParserSuccess(string name, string type, string domain) =>
        new AssertStatusSuccess<IParser>(
            () =>
                new LoggingNewParser(
                    _logger,
                    new StatusCachingNewParser(
                        new ValidatingNewParser(new ValidNewParser(new NewParser()))
                    )
                ).Register(new AddNewParser(name, type, domain))
        ).Asserted();

    public void AddNewParserFailure(string name, string type, string domain) =>
        new AssertStatusFailure<IParser>(
            () =>
                new LoggingNewParser(
                    _logger,
                    new StatusCachingNewParser(
                        new ValidatingNewParser(new ValidNewParser(new NewParser()))
                    )
                ).Register(new AddNewParser(name, type, domain))
        ).Asserted();

    public async Task<IParser> AsyncAddNewParserSuccess(string name, string type, string domain)
    {
        Status<IParser> created = await new AsyncAssertStatusSuccess<IParser>(
            () =>
                new AsyncStatusCachingNewParser(
                    new AsyncLoggingNewParser(
                        _logger,
                        new AsyncValidatingNewParser(
                            new AsyncSqlSpeakingNewParser(
                                _parsers,
                                new AsyncNewParser(
                                    new StatusCachingNewParser(
                                        new LoggingNewParser(
                                            _logger,
                                            new ValidatingNewParser(new NewParser())
                                        )
                                    )
                                )
                            )
                        )
                    )
                ).Register(new AsyncAddNewParser(name, type, domain))
        ).AsyncAsserted();
        return created.Value;
    }

    public async Task AsyncAddNewParserFailure(string name, string type, string domain) =>
        await new AsyncAssertStatusFailure<IParser>(
            () =>
                new AsyncStatusCachingNewParser(
                    new AsyncLoggingNewParser(
                        _logger,
                        new AsyncValidatingNewParser(
                            new AsyncSqlSpeakingNewParser(
                                _parsers,
                                new AsyncNewParser(
                                    new StatusCachingNewParser(
                                        new LoggingNewParser(
                                            _logger,
                                            new ValidatingNewParser(new NewParser())
                                        )
                                    )
                                )
                            )
                        )
                    )
                ).Register(new AsyncAddNewParser(name, type, domain))
        ).AsyncAsserted();

    public IParser CreateInitialParser()
    {
        Status<IParser> parser = new AssertStatusSuccess<Status<IParser>>(
            () =>
                new LoggingNewParser(
                    _logger,
                    new StatusCachingNewParser(
                        new ValidatingNewParser(new ValidNewParser(new NewParser()))
                    )
                ).Register(new AddNewParser("Test Parser", "Техника", "Test"))
        ).Asserted();
        return parser.Value;
    }

    public IParser CreateInitialParser(string name, string type, string domain)
    {
        Status<IParser> parser = new AssertStatusSuccess<Status<IParser>>(
            () =>
                new LoggingNewParser(
                    _logger,
                    new StatusCachingNewParser(
                        new ValidatingNewParser(new ValidNewParser(new NewParser()))
                    )
                ).Register(new AddNewParser(name, type, domain))
        ).Asserted();
        return parser.Value;
    }

    public async Task<IParser> CreateInitialParserAsync()
    {
        Status<IParser> created = await new AsyncAssertStatusSuccess<IParser>(
            () =>
                new AsyncStatusCachingNewParser(
                    new AsyncLoggingNewParser(
                        _logger,
                        new AsyncValidatingNewParser(
                            new AsyncSqlSpeakingNewParser(
                                _parsers,
                                new AsyncNewParser(
                                    new StatusCachingNewParser(
                                        new LoggingNewParser(
                                            _logger,
                                            new ValidatingNewParser(
                                                new ValidNewParser(new NewParser())
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                ).Register(new AsyncAddNewParser("Test Parser", "Техника", "Test"))
        ).AsyncAsserted();
        return created.Value;
    }

    public async Task<IParser> CreateInitialParserAsync(string name, string type, string domain)
    {
        Status<IParser> created = await new AsyncAssertStatusSuccess<IParser>(
            () =>
                new AsyncStatusCachingNewParser(
                    new AsyncLoggingNewParser(
                        _logger,
                        new AsyncValidatingNewParser(
                            new AsyncSqlSpeakingNewParser(
                                _parsers,
                                new AsyncNewParser(
                                    new StatusCachingNewParser(
                                        new LoggingNewParser(
                                            _logger,
                                            new ValidatingNewParser(new NewParser())
                                        )
                                    )
                                )
                            )
                        )
                    )
                ).Register(new AsyncAddNewParser(name, type, domain))
        ).AsyncAsserted();
        return created.Value;
    }

    public async Task UpdateParserAsyncFailure(
        IParser parser,
        string? state = null,
        int? waitDays = null
    ) =>
        await new AsyncAssertStatusFailure<IParser>(
            () =>
                new AsyncLoggingUpdatedParser(
                    _logger,
                    new AsyncStatusCachingUpdatedParser(
                        new AsyncValidatingUpdatedParser(
                            new AsyncSqlSpeakingUpdatedParser(
                                _parsers,
                                new AsyncUpdatedParser(
                                    new LoggingUpdatedParser(
                                        _logger,
                                        new StatusCachingUpdatedParser(
                                            new ValidatingUpdatedParser(new UpdatedParser())
                                        )
                                    )
                                )
                            )
                        )
                    )
                ).Update(new AsyncUpdateParser(parser.Identification().ReadId(), state, waitDays))
        ).AsyncAsserted();

    public async Task<IParser> UpdateParserAsyncSuccess(
        IParser parser,
        string? state = null,
        int? waitDays = null
    )
    {
        Status<IParser> updated = await new AsyncAssertStatusSuccess<IParser>(
            () =>
                new AsyncLoggingUpdatedParser(
                    _logger,
                    new AsyncStatusCachingUpdatedParser(
                        new AsyncValidatingUpdatedParser(
                            new AsyncSqlSpeakingUpdatedParser(
                                _parsers,
                                new AsyncUpdatedParser(
                                    new LoggingUpdatedParser(
                                        _logger,
                                        new StatusCachingUpdatedParser(
                                            new ValidatingUpdatedParser(new UpdatedParser())
                                        )
                                    )
                                )
                            )
                        )
                    )
                ).Update(new AsyncUpdateParser(parser.Identification().ReadId(), state, waitDays))
        ).AsyncAsserted();
        return updated.Value;
    }

    public async Task<IParser> UpdateParserAsyncSuccess(
        Guid parserId,
        string? state = null,
        int? waitDays = null
    )
    {
        Status<IParser> updated = await new AsyncAssertStatusSuccess<IParser>(
            () =>
                new AsyncLoggingUpdatedParser(
                    _logger,
                    new AsyncStatusCachingUpdatedParser(
                        new AsyncValidatingUpdatedParser(
                            new AsyncSqlSpeakingUpdatedParser(
                                _parsers,
                                new AsyncUpdatedParser(
                                    new LoggingUpdatedParser(
                                        _logger,
                                        new StatusCachingUpdatedParser(
                                            new ValidatingUpdatedParser(new UpdatedParser())
                                        )
                                    )
                                )
                            )
                        )
                    )
                ).Update(new AsyncUpdateParser(parserId, state, waitDays))
        ).AsyncAsserted();
        return updated.Value;
    }

    public async Task UpdateParserAsyncFailure(
        Guid parserId,
        string? state = null,
        int? waitDays = null
    ) =>
        await new AsyncAssertStatusFailure<IParser>(
            () =>
                new AsyncLoggingUpdatedParser(
                    _logger,
                    new AsyncStatusCachingUpdatedParser(
                        new AsyncValidatingUpdatedParser(
                            new AsyncSqlSpeakingUpdatedParser(
                                _parsers,
                                new AsyncUpdatedParser(
                                    new LoggingUpdatedParser(
                                        _logger,
                                        new StatusCachingUpdatedParser(
                                            new ValidatingUpdatedParser(new UpdatedParser())
                                        )
                                    )
                                )
                            )
                        )
                    )
                ).Update(new AsyncUpdateParser(parserId, state, waitDays))
        ).AsyncAsserted();

    public IParser UpdateParserSuccess(IParser parser, string? state = null, int? waitDays = null)
    {
        Status<IParser> updated = new AssertStatusSuccess<IParser>(
            () =>
                new LoggingUpdatedParser(
                    _logger,
                    new StatusCachingUpdatedParser(new ValidatingUpdatedParser(new UpdatedParser()))
                ).Updated(new UpdateParser(parser, state, waitDays))
        ).Asserted();
        return updated.Value;
    }

    public void UpdateParserFailure(IParser parser, string? state = null, int? waitDays = null) =>
        new AssertStatusFailure<IParser>(
            () =>
                new LoggingUpdatedParser(
                    _logger,
                    new StatusCachingUpdatedParser(new ValidatingUpdatedParser(new UpdatedParser()))
                ).Updated(new UpdateParser(parser, state, waitDays))
        ).Asserted();

    public IParser EnableParserSuccess(IParser parser)
    {
        Status<IParser> enabled = new AssertStatusSuccess<IParser>(
            () =>
                new LoggingEnabledParser(
                    _logger,
                    new StatusCachingEnabledParser(new EnabledParser())
                ).Enable(new EnableParser(parser))
        ).Asserted();
        return enabled.Value;
    }

    public void EnableParserFailure(IParser parser) =>
        new AssertStatusFailure<IParser>(
            () =>
                new LoggingEnabledParser(
                    _logger,
                    new StatusCachingEnabledParser(new EnabledParser())
                ).Enable(new EnableParser(parser))
        ).Asserted();

    public async Task<IParser> EnableParserSuccessAsync(IParser parser)
    {
        Status<IParser> status = await new AsyncAssertStatusSuccess<IParser>(
            () =>
                new AsyncLoggingEnabledParser(
                    _logger,
                    new AsyncStatusCachingEnabledParser(
                        new AsyncValidatingEnabledParser(
                            new AsyncSqlSpeakingEnabledParser(
                                _parsers,
                                new AsyncEnabledParser(
                                    new LoggingEnabledParser(
                                        _logger,
                                        new StatusCachingEnabledParser(new EnabledParser())
                                    )
                                )
                            )
                        )
                    )
                ).EnableAsync(
                    new AsyncEnableParser(parser.Identification().ReadId()),
                    CancellationToken.None
                )
        ).AsyncAsserted();
        return status.Value;
    }

    public async Task EnableParserFailureAsync(IParser parser) =>
        await new AsyncAssertStatusFailure<IParser>(
            () =>
                new AsyncLoggingEnabledParser(
                    _logger,
                    new AsyncStatusCachingEnabledParser(
                        new AsyncValidatingEnabledParser(
                            new AsyncSqlSpeakingEnabledParser(
                                _parsers,
                                new AsyncEnabledParser(
                                    new LoggingEnabledParser(
                                        _logger,
                                        new StatusCachingEnabledParser(new EnabledParser())
                                    )
                                )
                            )
                        )
                    )
                ).EnableAsync(
                    new AsyncEnableParser(parser.Identification().ReadId()),
                    CancellationToken.None
                )
        ).AsyncAsserted();

    public IParser DisableParserSuccess(IParser parser)
    {
        Status<IParser> disabled = new AssertStatusSuccess<IParser>(
            () =>
                new LoggingDisabledParser(
                    _logger,
                    new StatusCachingDisabledParser(new DisabledParser())
                ).Disable(new DisableParser(parser))
        ).Asserted();
        return disabled.Value;
    }

    public void DisableParserFailure(IParser parser) =>
        new AssertStatusFailure<IParser>(
            () =>
                new LoggingDisabledParser(
                    _logger,
                    new StatusCachingDisabledParser(new DisabledParser())
                ).Disable(new DisableParser(parser))
        ).Asserted();

    public async Task<IParser> DisableParserSuccessAsync(Guid id)
    {
        Status<IParser> parser = await new AsyncAssertStatusSuccess<IParser>(
            () =>
                new AsyncLoggingDisabledParser(
                    _logger,
                    new AsyncStatusCachingDisabledParser(
                        new AsyncValidatingDisabledParser(
                            new AsyncSqlSpeakingDisabledParser(
                                _parsers,
                                new AsyncDisabledParser(
                                    new LoggingDisabledParser(
                                        _logger,
                                        new StatusCachingDisabledParser(new DisabledParser())
                                    )
                                )
                            )
                        )
                    )
                ).Disable(new AsyncDisableParser(id), CancellationToken.None)
        ).AsyncAsserted();
        return parser.Value;
    }

    public async Task DisableParserFailureAsync(Guid id) =>
        await new AsyncAssertStatusFailure<IParser>(
            () =>
                new AsyncLoggingDisabledParser(
                    _logger,
                    new AsyncStatusCachingDisabledParser(
                        new AsyncValidatingDisabledParser(
                            new AsyncSqlSpeakingDisabledParser(
                                _parsers,
                                new AsyncDisabledParser(
                                    new LoggingDisabledParser(
                                        _logger,
                                        new StatusCachingDisabledParser(new DisabledParser())
                                    )
                                )
                            )
                        )
                    )
                ).Disable(new AsyncDisableParser(id), CancellationToken.None)
        ).AsyncAsserted();

    public IParserLink AddLinkSuccess(IParser parser, string? name, string? url)
    {
        Status<IParserLink> link = new AssertStatusSuccess<IParserLink>(
            () =>
                new LoggingNewParserLink(
                    _logger,
                    new ValidatingNewParserLink(new NewParserLink())
                ).Register(new AddParserLink(parser, name, url))
        ).Asserted();
        return link.Value;
    }

    public async Task<IParserLink> AddLinkSuccessAsync(Guid? parserId, string? name, string? url)
    {
        Status<IParserLink> link = await new AsyncAssertStatusSuccess<IParserLink>(
            () =>
                new AsyncLoggingNewParserLink(
                    _logger,
                    new AsyncValidatingNewParserLink(
                        new AsyncSqlSpeakingNewParserLink(
                            _parsers,
                            new AsyncNewParserLink(
                                new LoggingNewParserLink(
                                    _logger,
                                    new ValidatingNewParserLink(new NewParserLink())
                                )
                            )
                        )
                    )
                ).AsyncNew(new AsyncAddParserLink(parserId, name, url))
        ).AsyncAsserted();
        return link.Value;
    }

    public async Task AddLinkFailureAsync(Guid? parserId, string? name, string? url) =>
        await new AsyncAssertStatusFailure<IParserLink>(
            () =>
                new AsyncLoggingNewParserLink(
                    _logger,
                    new AsyncValidatingNewParserLink(
                        new AsyncSqlSpeakingNewParserLink(
                            _parsers,
                            new AsyncNewParserLink(
                                new LoggingNewParserLink(
                                    _logger,
                                    new ValidatingNewParserLink(new NewParserLink())
                                )
                            )
                        )
                    )
                ).AsyncNew(new AsyncAddParserLink(parserId, name, url))
        ).AsyncAsserted();

    public Status<IParserLink> AddLinkFailure(IParser parser, string? name, string? url) =>
        new AssertStatusFailure<IParserLink>(
            () =>
                new LoggingNewParserLink(
                    _logger,
                    new ValidatingNewParserLink(new NewParserLink())
                ).Register(new AddParserLink(parser, name, url))
        ).Asserted();

    public IParserLink RemoveLinkSuccess(IParser parser, Guid? linkId)
    {
        Status<IParserLink> link = new AssertStatusSuccess<IParserLink>(
            () =>
                new LoggingRemovedParserLink(
                    _logger,
                    new ValidatingRemovedParserLink(new RemovedParserLink())
                ).Removed(new RemoveParserLink(parser, linkId))
        ).Asserted();
        return link.Value;
    }

    public async Task<IParserLink> AsyncRemoveLinkSuccess(Guid? parserId, Guid? linkId)
    {
        Status<IParserLink> link = await new AsyncAssertStatusSuccess<IParserLink>(
            () =>
                new AsyncLoggingRemovedParserLink(
                    _logger,
                    new AsyncValidatingRemovedParserLink(
                        new AsyncSqlSpeakingRemovedParserLink(
                            _parsers,
                            new AsyncRemovedParserLink(
                                new LoggingRemovedParserLink(
                                    _logger,
                                    new ValidatingRemovedParserLink(new RemovedParserLink())
                                )
                            )
                        )
                    )
                ).AsyncRemoved(new AsyncRemoveParserLink(parserId, linkId))
        ).AsyncAsserted();
        return link.Value;
    }

    public async Task AsyncRemoveLinkFailure(Guid? parserId, Guid? linkId) =>
        await new AsyncAssertStatusFailure<IParserLink>(
            () =>
                new AsyncLoggingRemovedParserLink(
                    _logger,
                    new AsyncValidatingRemovedParserLink(
                        new AsyncSqlSpeakingRemovedParserLink(
                            _parsers,
                            new AsyncRemovedParserLink(
                                new LoggingRemovedParserLink(
                                    _logger,
                                    new ValidatingRemovedParserLink(new RemovedParserLink())
                                )
                            )
                        )
                    )
                ).AsyncRemoved(new AsyncRemoveParserLink(parserId, linkId))
        ).AsyncAsserted();

    public IParserLink RemoveLinkSuccess(IParser parser, IParserLink linkToRemove)
    {
        Status<IParserLink> link = new AssertStatusSuccess<IParserLink>(
            () =>
                new LoggingRemovedParserLink(
                    _logger,
                    new ValidatingRemovedParserLink(new RemovedParserLink())
                ).Removed(new RemoveParserLink(parser, linkToRemove))
        ).Asserted();
        return link.Value;
    }

    public void RemoveLinkFailure(IParser parser, Guid? linkId) =>
        new AssertStatusFailure<IParserLink>(
            () =>
                new LoggingRemovedParserLink(
                    _logger,
                    new ValidatingRemovedParserLink(new RemovedParserLink())
                ).Removed(new RemoveParserLink(parser, linkId))
        ).Asserted();

    public void RemoveLinkFailure(IParser parser, IParserLink link) =>
        new AssertStatusFailure<IParserLink>(
            () =>
                new LoggingRemovedParserLink(
                    _logger,
                    new ValidatingRemovedParserLink(new RemovedParserLink())
                ).Removed(new RemoveParserLink(parser, link))
        ).Asserted();

    public ParserStatisticsIncreasement IncreaseProcessedSuccess(IParser parser, IParserLink link)
    {
        Status<ParserStatisticsIncreasement> increasement =
            new AssertStatusSuccess<ParserStatisticsIncreasement>(
                () =>
                    new LoggingIncreasedProcessed(
                        _logger,
                        new ValidatingIncreasedProcessed(new IncreasedProcessed())
                    ).IncreaseProcessed(new IncreaseProcessed(parser, link))
            ).Asserted();
        return increasement.Value;
    }

    public async Task AsyncIncreaseProcessedSuccess(Guid? parserId, Guid? linkId) =>
        await new AsyncAssertStatusSuccess<ParserStatisticsIncreasement>(
            () =>
                new AsyncLoggingIncreaseProcessed(
                    _logger,
                    new AsyncValidatingIncreaseProcessed(
                        new AsyncSqlSpeakingIncreaseProcessed(
                            _parsers,
                            new AsyncIncreaseProcessed(
                                new LoggingIncreasedProcessed(
                                    _logger,
                                    new ValidatingIncreasedProcessed(new IncreasedProcessed())
                                )
                            )
                        )
                    )
                ).Increase(new AsyncIncreaseProcess(parserId, linkId))
        ).AsyncAsserted();

    public async Task AsyncIncreaseProcessedFailure(Guid? parserId, Guid? linkId) =>
        await new AsyncAssertStatusFailure<ParserStatisticsIncreasement>(
            () =>
                new AsyncLoggingIncreaseProcessed(
                    _logger,
                    new AsyncValidatingIncreaseProcessed(
                        new AsyncSqlSpeakingIncreaseProcessed(
                            _parsers,
                            new AsyncIncreaseProcessed(
                                new LoggingIncreasedProcessed(
                                    _logger,
                                    new ValidatingIncreasedProcessed(new IncreasedProcessed())
                                )
                            )
                        )
                    )
                ).Increase(new AsyncIncreaseProcess(parserId, linkId))
        ).AsyncAsserted();

    public ParserStatisticsIncreasement IncreaseProcessedSuccess(IParser parser, Guid? linkId)
    {
        Status<ParserStatisticsIncreasement> increasement =
            new AssertStatusSuccess<ParserStatisticsIncreasement>(
                () =>
                    new LoggingIncreasedProcessed(
                        _logger,
                        new ValidatingIncreasedProcessed(new IncreasedProcessed())
                    ).IncreaseProcessed(new IncreaseProcessed(parser, linkId))
            ).Asserted();
        return increasement.Value;
    }

    public void IncreaseProcessedFailure(IParser parser, IParserLink link) =>
        new AssertStatusFailure<ParserStatisticsIncreasement>(
            () =>
                new LoggingIncreasedProcessed(
                    _logger,
                    new ValidatingIncreasedProcessed(new IncreasedProcessed())
                ).IncreaseProcessed(new IncreaseProcessed(parser, link))
        ).Asserted();

    public void IncreaseProcessedFailure(IParser parser, Guid? linkId) =>
        new AssertStatusFailure<ParserStatisticsIncreasement>(
            () =>
                new LoggingIncreasedProcessed(
                    _logger,
                    new ValidatingIncreasedProcessed(new IncreasedProcessed())
                ).IncreaseProcessed(new IncreaseProcessed(parser, linkId))
        ).Asserted();

    public IParserLink ChangeLinkActivitySuccess(IParser parser, Guid? linkId, bool nextActivity)
    {
        Status<IParserLink> changed = new AssertStatusSuccess<IParserLink>(
            () =>
                new LoggingChangedLinkActivity(
                    _logger,
                    new ValidatingChangedLinkActivity(new ChangedLinkActivity())
                ).Changed(new ChangeLinkActivity(parser, linkId, nextActivity))
        ).Asserted();
        return changed.Value;
    }

    public async Task<IParserLink> ChangeLinkActivitySuccessAsync(
        Guid? parserId,
        Guid? linkId,
        bool nextActivity
    )
    {
        Status<IParserLink> link = await new AsyncAssertStatusSuccess<IParserLink>(
            () =>
                new AsyncLoggingChangedLinkActivity(
                    _logger,
                    new AsyncValidatingChangedLinkActivity(
                        new AsyncSqlSpeakingChangedLinkActivity(
                            _parsers,
                            new AsyncChangedLinkActivity(
                                new LoggingChangedLinkActivity(
                                    _logger,
                                    new ValidatingChangedLinkActivity(new ChangedLinkActivity())
                                )
                            )
                        )
                    )
                ).AsyncChangedActivity(new AsyncChangeLinkActivity(parserId, linkId, nextActivity))
        ).AsyncAsserted();
        return link.Value;
    }

    public async Task ChangeLinkActivityFailureAsync(
        Guid? parserId,
        Guid? linkId,
        bool nextActivity
    ) =>
        await new AsyncAssertStatusFailure<IParserLink>(
            () =>
                new AsyncLoggingChangedLinkActivity(
                    _logger,
                    new AsyncValidatingChangedLinkActivity(
                        new AsyncSqlSpeakingChangedLinkActivity(
                            _parsers,
                            new AsyncChangedLinkActivity(
                                new LoggingChangedLinkActivity(
                                    _logger,
                                    new ValidatingChangedLinkActivity(new ChangedLinkActivity())
                                )
                            )
                        )
                    )
                ).AsyncChangedActivity(new AsyncChangeLinkActivity(parserId, linkId, nextActivity))
        ).AsyncAsserted();

    public IParserLink ChangeLinkActivitySuccess(
        IParser parser,
        IParserLink link,
        bool nextActivity
    )
    {
        Status<IParserLink> changed = new AssertStatusSuccess<IParserLink>(
            () =>
                new LoggingChangedLinkActivity(
                    _logger,
                    new ValidatingChangedLinkActivity(new ChangedLinkActivity())
                ).Changed(new ChangeLinkActivity(parser, link, nextActivity))
        ).Asserted();
        return changed.Value;
    }

    public void ChangeLinkActivityFailure(IParser parser, Guid? linkId, bool nextActivity) =>
        new AssertStatusFailure<IParserLink>(
            () =>
                new LoggingChangedLinkActivity(
                    _logger,
                    new ValidatingChangedLinkActivity(new ChangedLinkActivity())
                ).Changed(new ChangeLinkActivity(parser, linkId, nextActivity))
        ).Asserted();

    public void ChangeLinkActivityFailure(IParser parser, IParserLink link, bool nextActivity) =>
        new AssertStatusFailure<IParserLink>(
            () =>
                new LoggingChangedLinkActivity(
                    _logger,
                    new ValidatingChangedLinkActivity(new ChangedLinkActivity())
                ).Changed(new ChangeLinkActivity(parser, link, nextActivity))
        ).Asserted();

    public IParserLink FinishLinkSuccess(IParser parser, IParserLink link, long? elapsed)
    {
        Status<IParserLink> finished = new AssertStatusSuccess<IParserLink>(
            () =>
                new LoggingFinishedParserLink(
                    _logger,
                    new ValidatingFinishedParserLink(new FinishedParserLink())
                ).Finished(new FinishParserLink(parser, link.Identification().ReadId(), elapsed))
        ).Asserted();
        return finished.Value;
    }

    public async Task<IParserLink> AsyncFinishLinkSuccess(
        Guid? parserId,
        Guid? linkId,
        long? elapsed
    )
    {
        Status<IParserLink> link = await new AsyncAssertStatusSuccess<IParserLink>(
            () =>
                new AsyncLoggingFinishedParserLink(
                    _logger,
                    new AsyncValidatingFinishedParserLink(
                        new AsyncSqlSpeakingFinishedParserLink(
                            _parsers,
                            new AsyncFinishedParserLink(
                                new LoggingFinishedParserLink(
                                    _logger,
                                    new ValidatingFinishedParserLink(new FinishedParserLink())
                                )
                            )
                        )
                    )
                ).AsyncFinished(new AsyncFinishParserLink(parserId, linkId, elapsed))
        ).AsyncAsserted();
        return link.Value;
    }

    public async Task AsyncFinishLinkFailure(Guid? parserId, Guid? linkId, long? elapsed) =>
        await new AsyncAssertStatusFailure<IParserLink>(
            () =>
                new AsyncLoggingFinishedParserLink(
                    _logger,
                    new AsyncValidatingFinishedParserLink(
                        new AsyncSqlSpeakingFinishedParserLink(
                            _parsers,
                            new AsyncFinishedParserLink(
                                new LoggingFinishedParserLink(
                                    _logger,
                                    new ValidatingFinishedParserLink(new FinishedParserLink())
                                )
                            )
                        )
                    )
                ).AsyncFinished(new AsyncFinishParserLink(parserId, linkId, elapsed))
        ).AsyncAsserted();

    public IParserLink FinishLinkSuccess(IParser parser, Guid? linkId, long? elapsed)
    {
        Status<IParserLink> finished = new AssertStatusSuccess<IParserLink>(
            () =>
                new LoggingFinishedParserLink(
                    _logger,
                    new ValidatingFinishedParserLink(new FinishedParserLink())
                ).Finished(new FinishParserLink(parser, linkId, elapsed))
        ).Asserted();
        return finished.Value;
    }

    public void FinishLinkFailure(IParser parser, IParserLink link, long? elapsed) =>
        new AssertStatusFailure<IParserLink>(
            () =>
                new LoggingFinishedParserLink(
                    _logger,
                    new ValidatingFinishedParserLink(new FinishedParserLink())
                ).Finished(new FinishParserLink(parser, link.Identification().ReadId(), elapsed))
        ).Asserted();

    public void FinishLinkFailure(IParser parser, Guid? linkId, long? elapsed) =>
        new AssertStatusFailure<IParserLink>(
            () =>
                new LoggingFinishedParserLink(
                    _logger,
                    new ValidatingFinishedParserLink(new FinishedParserLink())
                ).Finished(new FinishParserLink(parser, linkId, elapsed))
        ).Asserted();

    public IParser StartParserSuccess(IParser parser)
    {
        Status<IParser> started = new AssertStatusSuccess<IParser>(
            () =>
                new LoggingStartedParser(_logger, new StartedParser()).Started(
                    new StartParser(parser)
                )
        ).Asserted();
        return started.Value;
    }

    public async Task<IParser> StartParserSuccessAsync(Guid? parserId)
    {
        Status<IParser> started = await new AsyncAssertStatusSuccess<IParser>(
            () =>
                new AsyncLoggingStartedParser(
                    _logger,
                    new AsyncValidatingStartedParser(
                        new AsyncSqlSpeakingStartedParser(
                            _parsers,
                            new AsyncStartedParser(
                                new LoggingStartedParser(_logger, new StartedParser())
                            )
                        )
                    )
                ).StartedAsync(new AsyncStartParser(parserId))
        ).AsyncAsserted();
        return started.Value;
    }

    public async Task StartParserFailureAsync(Guid? parserId)
    {
        await new AsyncAssertStatusFailure<IParser>(
            () =>
                new AsyncLoggingStartedParser(
                    _logger,
                    new AsyncValidatingStartedParser(
                        new AsyncSqlSpeakingStartedParser(
                            _parsers,
                            new AsyncStartedParser(
                                new LoggingStartedParser(_logger, new StartedParser())
                            )
                        )
                    )
                ).StartedAsync(new AsyncStartParser(parserId))
        ).AsyncAsserted();
    }

    public void StartParserFailure(IParser parser) =>
        new AssertStatusFailure<IParser>(
            () =>
                new LoggingStartedParser(_logger, new StartedParser()).Started(
                    new StartParser(parser)
                )
        ).Asserted();

    public IParser StoppedParserSuccess(IParser parser)
    {
        Status<IParser> started = new AssertStatusSuccess<IParser>(
            () =>
                new LoggingStoppedParser(_logger, new StoppedParser()).Stopped(
                    new StopParser(parser)
                )
        ).Asserted();
        return started.Value;
    }

    public async Task<IParser> AsyncStoppedParserSuccess(Guid? parserId)
    {
        Status<IParser> parser = await new AsyncAssertStatusSuccess<IParser>(
            () =>
                new AsyncLoggingStoppedParser(
                    _logger,
                    new AsyncValidatingStoppedParser(
                        new AsyncSqlSpeakingStoppedParser(
                            _parsers,
                            new AsyncStoppedParser(
                                new LoggingStoppedParser(_logger, new StoppedParser())
                            )
                        )
                    )
                ).AsyncStopped(new AsyncStopParser(parserId))
        ).AsyncAsserted();
        return parser.Value;
    }

    public async Task AsyncStoppedParserFailure(Guid? parserId)
    {
        await new AsyncAssertStatusFailure<IParser>(
            () =>
                new AsyncLoggingStoppedParser(
                    _logger,
                    new AsyncValidatingStoppedParser(
                        new AsyncSqlSpeakingStoppedParser(
                            _parsers,
                            new AsyncStoppedParser(
                                new LoggingStoppedParser(_logger, new StoppedParser())
                            )
                        )
                    )
                ).AsyncStopped(new AsyncStopParser(parserId))
        ).AsyncAsserted();
    }

    public void StoppedParserFailure(IParser parser) =>
        new AssertStatusFailure<IParser>(
            () =>
                new LoggingStoppedParser(_logger, new StoppedParser()).Stopped(
                    new StopParser(parser)
                )
        ).Asserted();
}
