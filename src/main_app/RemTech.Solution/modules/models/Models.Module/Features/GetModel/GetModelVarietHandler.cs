using Models.Module.Types;
using Shared.Infrastructure.Module.Cqrs;

namespace Models.Module.Features.GetModel;

internal sealed class GetModelVarietHandler(Serilog.ILogger logger)
    : ICommandHandler<GetModelCommand, IModel>
{
    private readonly List<ICommandHandler<GetModelCommand, IModel>> _handlers = [];

    public GetModelVarietHandler With(ICommandHandler<GetModelCommand, IModel> handler)
    {
        _handlers.Add(handler);
        return this;
    }

    public async Task<IModel> Handle(GetModelCommand command, CancellationToken ct = default)
    {
        foreach (ICommandHandler<GetModelCommand, IModel> handler in _handlers)
        {
            try
            {
                IModel model = await handler.Handle(command, ct);
                logger.Information("Resolved model: {Name}", model.Name);
                return model;
            }
            catch
            {
                // ignored
            }
        }

        throw new UnableToResolveModelException();
    }
}
