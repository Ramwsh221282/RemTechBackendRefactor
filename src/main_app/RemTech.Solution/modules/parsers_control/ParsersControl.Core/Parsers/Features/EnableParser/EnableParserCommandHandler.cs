using ParsersControl.Core.Parsers.Contracts;
using ParsersControl.Core.Parsers.Models;
using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.InfrastructureContracts;

namespace ParsersControl.Core.Parsers.Features.EnableParser;

public sealed class EnableParserCommandHandler(
    ISubscribedParsersRepository repository,
    ITransactionSource transactionSource
)
    : ICommandHandler<EnableParserCommand, SubscribedParser>
{
    public async Task<Result<SubscribedParser>> Execute(
        EnableParserCommand command, 
        CancellationToken ct = default)
    {
        ITransactionScope scope = await transactionSource.BeginTransaction();
        Result<ISubscribedParser> parser = await GetRequiredParser(command, ct);
        Result<SubscribedParser> enabled = Enable(parser);
        Result saving = await SaveChanges(parser, scope, ct);
        if (saving.IsFailure) return saving.Error;
        return enabled;
    }

    private async Task<Result> SaveChanges(
        Result<ISubscribedParser> parser,
        ITransactionScope txn,
        CancellationToken ct)
    {
        if (parser.IsFailure) return Result.Failure(parser.Error);
        await repository.Save(parser.Value);
        return await txn.Commit(ct);
    }
    
    private Result<SubscribedParser> Enable(Result<ISubscribedParser> parser)
    {
        if (parser.IsFailure) return parser.Error;
        return parser.Value.Enable();
    }
    
    private async Task<Result<ISubscribedParser>> GetRequiredParser(
        EnableParserCommand command,
        CancellationToken ct)
    {
        SubscribedParserQuery query = new(Id: command.Id);
        Result<ISubscribedParser> parser = await repository.Get(query, ct: ct);
        return parser;
    }
}