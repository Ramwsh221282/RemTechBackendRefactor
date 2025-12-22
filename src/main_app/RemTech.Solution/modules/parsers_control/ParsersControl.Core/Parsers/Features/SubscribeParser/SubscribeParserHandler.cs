using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;

namespace ParsersControl.Core.Parsers.Features.SubscribeParser;

public sealed class SubscribeParserHandler(
    ISubscribedParsersRepository repository,
    IOnParserSubscribedListener listener) 
    : ICommandHandler<SubscribeParserCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(SubscribeParserCommand command)
    {
        Result<SubscribedParser> result = await ProcessParserSubscription(command);
        await NotifyListener(result);
        return result;
    }

    private async Task<Result<SubscribedParser>> ProcessParserSubscription(SubscribeParserCommand command)
    {
        SubscribedParserId id = SubscribedParserId.Create(command.Id).Value;
        SubscribedParserIdentity identity = SubscribedParserIdentity.Create(command.Domain, command.Type);
        return await SubscribedParser.CreateNew(id, identity, repository);
    }

    private async Task NotifyListener(Result<SubscribedParser> result)
    {
        if (result.IsFailure) return;
        await listener.Handle(result.Value);        
    }
}