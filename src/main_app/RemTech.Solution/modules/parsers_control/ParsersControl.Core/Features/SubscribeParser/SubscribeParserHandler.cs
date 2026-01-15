using ParsersControl.Core.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Decorators.Transactions;

namespace ParsersControl.Core.Features.SubscribeParser;

[TransactionalHandler]
public sealed class SubscribeParserHandler(
    ISubscribedParsersRepository repository,
    IOnParserSubscribedListener listener
) : ICommandHandler<SubscribeParserCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(
        SubscribeParserCommand command,
        CancellationToken ct = default
    )
    {
        Result<SubscribedParser> result = await ProcessParserSubscription(command, ct);
        await NotifyListener(result, ct);
        return result;
    }

    private async Task<Result<SubscribedParser>> ProcessParserSubscription(
        SubscribeParserCommand command,
        CancellationToken ct = default
    )
    {
        SubscribedParserId id = SubscribedParserId.Create(command.Id).Value;
        SubscribedParserIdentity identity = SubscribedParserIdentity.Create(
            command.Domain,
            command.Type
        );
        return await SubscribedParser.CreateNew(id, identity, repository, ct: ct);
    }

    private async Task NotifyListener(
        Result<SubscribedParser> result,
        CancellationToken ct = default
    )
    {
        if (result.IsFailure)
            return;
        await listener.Handle(result.Value, ct: ct);
    }
}
