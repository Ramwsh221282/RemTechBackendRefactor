using Models.Module.Features.GetModel;
using Models.Module.Types;
using Npgsql;
using RemTech.Core.Shared.Cqrs;
using Shared.Infrastructure.Module.Postgres.Embeddings;

namespace Models.Module.Public;

internal sealed class ModelPublicApi(
    NpgsqlDataSource dataSource,
    Serilog.ILogger logger,
    IEmbeddingGenerator generator
) : IModelPublicApi
{
    public async Task<ModelResponse> Get(string name, CancellationToken cancellationToken = default)
    {
        GetModelCommand command = new GetModelCommand(name);
        await using NpgsqlConnection connection = await dataSource.OpenConnectionAsync(
            cancellationToken
        );
        ICommandHandler<GetModelCommand, IModel> handler = new GetModelVarietHandler(logger)
            .With(new GetNewlyCreatedModelHandler(connection, generator))
            .With(new GetModelByNameHandler(connection))
            .With(new GetModelByEmbeddingHandler(connection, generator));
        IModel model = await handler.Handle(command, cancellationToken);
        return new ModelResponse(model.Id, model.Name, model.Rating);
    }
}
