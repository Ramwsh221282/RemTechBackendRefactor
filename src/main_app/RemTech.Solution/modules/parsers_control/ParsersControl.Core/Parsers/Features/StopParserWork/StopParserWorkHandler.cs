using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Parsers.Features.StopParserWork;

public sealed class StopParserWorkHandler(
    ITransactionSource transactionSource,
    ISubscribedParsersRepository repository
) : ICommandHandler<StopParserWorkCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(StopParserWorkCommand command)
    {
        ITransactionScope scope = await transactionSource.BeginTransaction();
        SubscribedParserQuery query = new(Id: command.Id, WithLock: true);
        
        Result<ISubscribedParser> parser = await repository.Get(query);
        if (parser.IsFailure) return parser.Error;
        
        Result<SubscribedParser> result = parser.Value.FinishWork();
        if (result.IsFailure) return result.Error;
        
        await repository.Save(parser.Value);
        Result commit = await scope.Commit();
        if (commit.IsFailure) return commit.Error;
        return result;
    }
}