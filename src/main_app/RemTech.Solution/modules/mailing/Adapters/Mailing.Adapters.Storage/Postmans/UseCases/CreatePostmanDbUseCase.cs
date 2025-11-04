using System.Data;
using Mailing.Domain.Postmans;
using Mailing.Domain.Postmans.Factories;
using Mailing.Domain.Postmans.UseCases.CreatePostman;
using RemTech.Core.Shared.Primitives.Async;
using RemTech.Core.Shared.Result;
using Shared.Infrastructure.Module.Postgres;

namespace Mailing.Adapters.Storage.Postmans.UseCases;

public sealed class CreatePostmanDbUseCase(
    ComposedAsync<ICreatePostmanUseCase, IPostman> delayedExecution,
    PostgresDatabase database,
    CancellationToken ct,
    ICreatePostmanUseCase useCase) : CreatePostmanUseCaseEnvelope(useCase)
{
    public override Status<IPostman> Create(PostmanConstructionContext context)
    {
        Status<IPostman> result = base.Create(context);
        if (result.IsFailure)
            return result.Error;

        IPostman postman = result.Value;
        delayedExecution.Add(new Async<ICreatePostmanUseCase, Status<IPostman>>(async _ => await Insert(postman)));
        return result;
    }

    private async Task<Status<IPostman>> Insert(IPostman postman)
    {
        using IDbConnection connection = await database.ProvideConnection(ct);
        DbPostman dbPostman = new DbPostman(connection, postman);

        if (await dbPostman.HasUniqueEmail(ct) == false)
            return Error.Conflict($"Почтовый сервис с почтой: {dbPostman.Email} уже существует.");

        await dbPostman.Save(ct);
        return Status<IPostman>.Success(postman);
    }
}